using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Database.Streaming;
using FirebaseWorkout.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseWorkout.Service.DBService.Firebase
{
	// Repository של משתמשים - אחראי על כל הפעולות של משתמשים
	// מול Firebase Auth (אימות) ו-Realtime Database (נתונים)
	public class FirebaseUsersRepository : FirebaseRealtimeService, IAppUserRepository
	{
		// שירות אימות לכניסה/הרשמה/מחיקה מ-Firebase Auth
		private IAuthService _authService;
		// שירות לוגים
		private IAppLogger _appLogger;

		// הקונסטרקטור מקבל שירותים דרך DI
		public FirebaseUsersRepository(IAuthService authService, IAppLogger appLogger)
		{
			_authService = authService;
			_appLogger = appLogger;
		}

		// כניסה למערכת - מאמת מול Auth ושולף את פרטי המשתמש מ-Database
		public async Task<AppUser> SignInAsync(string userEmail, string userPassword)
		{
			try
			{
				// שלב 1: אימות מול Firebase Auth - מקבל את ה-User ID
				string userId = await _authService.SignIn(userEmail, userPassword);
				// שלב 2: שליפת פרטי המשתמש מ-Realtime Database
				AppUser appUser = await GetUserByIdAsync(userId);
				_appLogger.LogDebug($"FirebaseUsersRepository {userEmail} SignIn successfully");
				return appUser;
			}
			catch (Exception ex)
			{
				_appLogger.LogDebug($"FirebaseUsersRepository SignIn failed: {ex.Message}");
				if (!ex.Message.Contains("Incorrect email or password"))
					throw new Exception("SignIn failed!");

				throw new Exception(ex.Message);
			}
		}
		// הרשמת משתמש חדש - יוצר ב-Auth וגם שומר ב-Database
		public async Task<string> CreateAsync(AppUser appUser)
		{
			try
			{
				// שלב 1: יצירת המשתמש ב-Firebase Auth
				string userId = await _authService.CreateAuth(appUser.UserEmail!, appUser.UserPassword!);
				appUser.Id = userId;
				// שלב 2: שמירת פרטי המשתמש ב-Realtime Database
				await RegisterAppUser(appUser);
				_appLogger.LogDebug($"FirebaseUsersRepository {appUser.UserEmail} SignUp successfully");
				return userId;
			}
			catch (Exception ex)
			{
				_appLogger.LogDebug($"FirebaseUsersRepository SignIn failed: {ex.Message}");
				if (!ex.Message.Contains("RealTimeDB"))
					throw new Exception(ex.Message);

				throw new Exception("SignUp new user failed!");
			}
		}
		// מחיקת משתמש - גם מ-Auth וגם מ-Database
		public async Task DeleteAsync(AppUser appUser)
		{
			try
			{
				//1 Delete user data from Firebase Auth module
				await _authService.RemoveAuth(appUser.UserEmail!, appUser.UserPassword!);

				//2 Delete user data from Realtime Database
				await _firebaseClient!
					.Child("users")
					.Child(appUser.Id)
					.DeleteAsync();
				_appLogger.LogDebug($"FirebaseUsersRepository Delete User {appUser.UserEmail} successfully.");
			}
			catch (Exception ex)
			{
				_appLogger.LogDebug($"FirebaseUsersRepository Delete User {appUser.UserEmail} failed: {ex.Message}");
				throw new Exception("Delete user failed!");
			}
		}
		public List<AppUser> GetAllAsync()
		{
			throw new NotImplementedException();
		}
		// שליפת משתמש לפי מזהה מ-Realtime Database
		public async Task<AppUser> GetUserByIdAsync(string userId)
		{
			string errorMessage = string.Empty;
			try
			{
				var user = await _firebaseClient!
					.Child("users")
					.Child(userId) //using Firebase.Database.Query;
					.OnceSingleAsync<AppUser>();

				return user;
			}
			catch (FirebaseException ex)
			{
				if (ex.Message.Contains("401") || ex.Message.Contains("Permission denied"))
				{
					errorMessage = "GetUserByIdAsync failed: Permissions denied!";
				}
				else if (ex.Message.Contains("404"))
				{
					errorMessage = "GetUserByIdAsync failed: Wrong db path!";
				}
				else
				{
					errorMessage = "GetUserByIdAsync failed: Unknown exception!";
				}

				_appLogger.LogDebug($"FirebaseUsersRepository {errorMessage}");
				throw new Exception(errorMessage);
			}
			catch (Exception ex)
			{			
				throw new Exception($"FirebaseUsersRepository GetUserByIdAsync failed! {ex.Message}");
			}

			//var users = await  
			//.Child("Users")
			//.OrderBy("Id")
			//.EqualTo(userId)
			//.OnceAsync<AppUser>();

			//	להגדיר ב-Firebase Console תחת לשונית Rules אינדקס לשדה Id:
			//			{
			//				"rules": {
			//					"Users": {
			//						".indexOn": ["Id"]
			//					}
			//				}
			//			}	

			// מדפיס את תוכן התשובה מהשרת - כאן תראה את הסיבה האמיתית
			//Debug.WriteLine($"Database Error Content: {ex.ResponseContent}");
			//Debug.WriteLine($"Database Error Message: {ex.Message}");

			//string userMessage = "אירעה שגיאה בתקשורת עם בסיס הנתונים.";

			//if (ex.Message.Contains("401") || ex.ResponseContent.Contains("Permission denied"))
			//{
			//	userMessage = "אין לך הרשאות לבצע את הפעולה הזו (בדוק את ה-Rules).";
			//}
			//else if (ex.Message.Contains("404"))
			//{
			//	userMessage = "הנתיב בבסיס הנתונים לא נמצא.";
			//}
		}
		// עדכון פרטי משתמש (שם, טלפון) ב-Database
		public async Task UpdateAsync(AppUser appUser)
		{
			try
			{
				await _firebaseClient!
					.Child("users")
					.Child(appUser.Id)
					.PatchAsync(new { FirstName = appUser.FirstName,
									  LastName = appUser.LastName,									  
									  UserMobile = appUser.UserMobile
					}); 

				_appLogger.LogDebug($"Update user {appUser.UserEmail} detailes successfully.");
			}
			catch (Exception ex)
			{
				_appLogger.LogDebug($"Error updating user details: {ex.Message}");
				throw new Exception("Update failed!");
			}
		}
		// שמירת אובייקט משתמש ב-Realtime Database תחת users/[id]
		public async Task RegisterAppUser(AppUser appUser)
		{
			try
			{
				await _firebaseClient!
			   .Child("users")
			   .Child(appUser.Id)
			   .PutAsync(new AppUser()
			   {
				   Id = appUser.Id,
				   FirstName = appUser.FirstName,
				   LastName = appUser.LastName,
				   UserEmail = appUser.UserEmail,
				   UserPassword = appUser.UserPassword,
				   UserMobile = appUser.UserMobile,
				   RegDate = appUser.RegDate,
				   UBDate = appUser.UBDate,
				   IsAdmin = appUser.IsAdmin,
				   IsServicePerson = appUser.IsServicePerson
			   });				
			}
			catch (Exception ex)
			{
				_appLogger.LogDebug($"RealTimeDB SignUp failed: {ex.Message}");
				throw new Exception("RealTimeDB add new user failed");
			}
		}
		// הפיכת משתמש למנהל - מעדכן רק את שדה IsAdmin ל-true
		public async Task SetToAdmin(string userId)
		{
			try
			{
				await _firebaseClient!
					.Child("users")
					.Child(userId)
					.PatchAsync(new { IsAdmin = true }); // שולח רק את השדה הזה

				_appLogger.LogDebug("User admin status updated successfully.");
			}
			catch (Exception ex)
			{
				_appLogger.LogDebug($"Error updating field: {ex.Message}");
				throw new Exception("SetToAdmin failed!");
			}
		}
		// שליפת כל המשתמשים מ-Database עם מיפוי שדות מפורש
		public async Task<List<AppUser>> GetAllUserAsync()
		{
			try
			{
				var users = await _firebaseClient!
					.Child("users")
					.OnceAsync<AppUser>();

				//users - collection of Firebase objects => Convert to List<AppUser>
				return users.Select(u => new AppUser()
				{
					Id = u.Object.Id,
					FirstName = u.Object.FirstName,
					LastName = u.Object.LastName,
					UserEmail = u.Object.UserEmail,
					UserPassword = u.Object.UserPassword,
					RegDate = u.Object.RegDate,
					UBDate = u.Object.UBDate,
					IsAdmin = u.Object.IsAdmin,
					IsServicePerson = u.Object.IsServicePerson
				}).ToList();
			}
			catch (FirebaseException ex)
			{
				_appLogger.LogDebug($"GetAllUsers failed: {ex.Message}");
				return new List<AppUser>();
			}
		}
		// הרשמה לעדכונים בזמן אמת על שינויי משתמשים (Rx Observable)
		public IObservable<FirebaseEvent<AppUser>> SubscribeToUserChanges()
		{
			try
			{
				return _firebaseClient!
				.Child("users")
				.AsObservable<AppUser>();
				//.ObserveOn(System.Reactive.Concurrency.Scheduler.Default);
			}
			catch (Exception ex)
			{
				_appLogger.LogError("SubscribeToUserChanges failed: " + ex.Message);
				throw new Exception("SubscribeToUserChanges failed!");
			}
			
		}
	}
}

using Firebase.Auth;
using Firebase.Auth.Providers;
using FirebaseWorkout.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseWorkout.Service.DBService.Firebase
{
	// שירות אימות Firebase - מנהל כניסה, הרשמה ומחיקת משתמשים
	// מממש את IAuthService ומתקשר עם Firebase Authentication
	internal class FirebaseAuthService : IAuthService
	{
		// לקוח Firebase Auth לביצוע פעולות אימות
		private FirebaseAuthClient? _authClient;
		// שירות לוגים לרישום פעולות ושגיאות
		private IAppLogger _logger;

		// הקונסטרקטור מאתחל את חיבור Firebase Auth עם מפתח API
		public FirebaseAuthService(IAppLogger logger)
		{
			_logger = logger;

			// הגדרת Firebase Auth עם מפתח API ודומיין
			var config = new FirebaseAuthConfig()
			{
				ApiKey = "AIzaSyCZokKmPLS4tTHMvwXQ9GUkcDazSHzNCwg",
				AuthDomain = "qrcodedb-19df3.firebaseapp.com",
				Providers = new FirebaseAuthProvider[]
					{
						new EmailProvider()
					},
				//UserRepository = new FileUserRepository("AppCurrentUser") //Save login status localy
			};
			_authClient = new FirebaseAuthClient(config);
			_logger = logger;
		}

		// כניסה עם אימייל וסיסמה - מחזיר את ה-UID של המשתמש
		public async Task<string> SignIn(string userEmail, string userPassword)
		{
			string errorMessage = string.Empty;
			try
			{
				// ניסיון כניסה מול Firebase Auth
				await _authClient!.SignInWithEmailAndPasswordAsync(userEmail, userPassword);
				// החזרת מזהה המשתמש הייחודי
				return _authClient.User.Info.Uid;
			}
			catch (FirebaseAuthException ex)
			{
				// טיפול בשגיאת אימייל/סיסמה שגויים
				if (ex.Message.Contains("INVALID_LOGIN_CREDENTIALS"))
				{
					errorMessage = "Incorrect email or password!";
					_logger.LogDebug($" SignInAuth failed: {userEmail} {userPassword}, {errorMessage}");
				}
				else
				{
					errorMessage = "SignInAuth failed: Unknown exception!";
					_logger.LogDebug($"SignInAuth failed: {userEmail} {userPassword}, Unknown exception!");
				}
				throw new Exception(errorMessage);
			}
			catch (Exception ex)
			{
				_logger.LogDebug($"SignInAuth failed: {userEmail} {userPassword}, {ex.Message}");
				throw new Exception("SignIn failed!");
			}

		}
		// יצירת משתמש חדש ב-Firebase Auth - מחזיר את ה-UID
		public async Task<string> CreateAuth(string userEmail, string userPassword)
		{
			try
			{
				// יצירת משתמש חדש עם אימייל וסיסמה
				await _authClient!.CreateUserWithEmailAndPasswordAsync(userEmail, userPassword);
				_logger.LogDebug($"AppUser Auth {userEmail} created successfully");
				return _authClient.User.Uid;
			}
			catch (FirebaseAuthException ex)
			{
				string errorMessage = string.Empty;

				// בדיקת סוג השגיאה - אימייל לא תקין
				if (ex.Message.Contains("INVALID_EMAIL"))
				{
					errorMessage = "Invalid email adress!";
				}
				// בדיקת סוג השגיאה - אימייל כבר קיים
				if (ex.Message.Contains("EMAIL_EXISTS"))
				{
					errorMessage = "This email already exists!";
				}
				// בדיקת סוג השגיאה - סיסמה חלשה
				if (ex.Message.Contains("WEAK_PASSWORD"))
				{
					errorMessage = "Weak password!";
				}

				_logger.LogDebug($"CreateUserAuth failed: {ex.Message}");
				throw new Exception(errorMessage);

				//// Exception reason
				//AuthErrorReason reason = ex.Reason;

				//string errorMessage = reason switch
				//{
				//	AuthErrorReason.InvalidEmailAddress => "Error: Incorrect email adress", // "כתובת האימייל לא תקינה",
				//	AuthErrorReason.WrongPassword => "Error: Incorrect password", // "סיסמה שגויה",					
				//	AuthErrorReason.EmailExists => "Error: This email allready exist", //"האימייל כבר רשום במערכת",
				//	_ => "Error: Unknown exception" // "אירעה שגיאה לא ידועה"
				//};

				//_appLogger.LogDebug($"Firebase Auth creation failed: {errorMessage}");				
			}
			catch (Exception ex)
			{
				_logger.LogDebug($"CreateUserAuth failed: {ex.Message}");
				return "SignUp new user failed!";
			}
		}
		// מחיקת משתמש מ-Firebase Auth
		// דורש כניסה מחדש של המנהל אחרי המחיקה
		public async Task RemoveAuth(string userEmail, string userPassword)
		{
			try
			{
				// שלב 1: כניסה עם המשתמש שרוצים למחוק
				await _authClient!.SignInWithEmailAndPasswordAsync(userEmail, userPassword);
				// שלב 2: מחיקת המשתמש מ-Auth
				await _authClient.User.DeleteAsync();
				// שלב 3: כניסה מחדש עם המשתמש המחובר (המנהל)
				await _authClient!.SignInWithEmailAndPasswordAsync(
					(App.Current as App)!.CurrentUser!.UserEmail,
					(App.Current as App)!.CurrentUser!.UserPassword);

				_logger.LogDebug($"User {userEmail} removed from Auth successfully");
			}
			catch (Exception ex)
			{
				_logger.LogDebug($"Remove user {userEmail} from Auth failed: {ex.Message}");
				throw new Exception("Remove user from Auth failed!");
			}
		}

		public async Task SignOut()
		{
			throw new NotImplementedException();
		}
	}
}

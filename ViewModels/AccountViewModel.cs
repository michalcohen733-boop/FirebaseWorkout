using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using FirebaseWorkout.Helper;
using FirebaseWorkout.Model;
using FirebaseWorkout.Service;
using FirebaseWorkout.Service.DBService;
using FirebaseWorkout.Service.DBService.Firebase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseWorkout.ViewModels
{
	// ViewModel של מסך חשבון - מציג ומאפשר עריכת פרטי המשתמש
	// מממש IQueryAttributable - יכול לקבל משתמש מרשימת המשתמשים (מנהל) או להציג את המשתמש הנוכחי
	public partial class AccountViewModel : ObservableObject, IQueryAttributable
	{
		// שירות התראות
		IAlertService _alertService;
		// Repository משתמשים
		private readonly IAppUserRepository _dbService;

		#region Fields
		// שם פרטי
		[ObservableProperty]
		private string _firstName;

		// שם משפחה
		[ObservableProperty]
		private string _lastName;

		// כתובת אימייל
		[ObservableProperty]
		private string _userEmail;

		// מספר טלפון נייד
		[ObservableProperty]
		private string _userMobile;

		// משתמש שהתקבל מרשימת המשתמשים (מסך מנהל), או null אם זה המשתמש הנוכחי
		[ObservableProperty]
		private AppUser _recievedUser;

		// האם להציג כפתור מחיקה (רק אם מנהל צופה במשתמש אחר)
		[ObservableProperty]
		private bool _isDeleteButtonVisible;

		// אייקון מחיקה
		[ObservableProperty]
		private string _deleteIcon;

		// האם להציג הודעת שגיאה
		[ObservableProperty]
		private bool _errorMessageIsVisible;

		// הודעת שגיאה
		[ObservableProperty]
		private string _errorMessage;

		// האם מציג מסך טעינה
		[ObservableProperty]
		private bool _isBusy;

		// תמונת משתמש בפורמט Base64
		[ObservableProperty]
		private string _userImageBase64;

		// מקור התמונה להצגה ב-UI
		[ObservableProperty]
		private ImageSource _userImageSource;
		#endregion

		//public ImageSource UserImageSource => ImageSource.FromStream(() =>
		//{
		//	byte[] bytes = Convert.FromBase64String(UserImageBase64);
		//	return new MemoryStream(bytes);
		//});


		// הקונסטרקטור מקבל שירותים דרך DI
		public AccountViewModel(IAppUserRepository dbService, IAlertService alertService)
		{	
			_alertService = alertService;
			_dbService = dbService;
			DeleteIcon = FontHelper.DELETE_USER_ICON;
			IsDeleteButtonVisible = false; // Initially hide the delete button	
			if(!string.IsNullOrEmpty(UserImageBase64))
			{
				_userImageSource = ImageSource.FromStream(() =>
				{
					byte[] bytes = Convert.FromBase64String(UserImageBase64);
					return new MemoryStream(bytes);
				});
			}		
		}

		// מחיקת משתמש - מציג אישור ומוחק מ-Firebase Auth ו-Database
		[RelayCommand]
		private async Task Delete()
		{
			bool confirm = await Shell.Current.DisplayAlert(
					"Admin",
					"Are you sure you want to delete this user?",
					"Yes",
					"No"
				);

			if (confirm) //Delete User from database
			{
				try
				{
					IsBusy = true;
					await _dbService.DeleteAsync(RecievedUser);
					await Shell.Current.GoToAsync(".."); // Navigate back to the previous page
					IsBusy = false;
				}
				catch (Exception ex)
				{
					IsBusy = false;
					await _alertService.ShowAlertAsync("KASATA", ex.Message, "OK");
				}
			}
		}

		// עדכון פרטי משתמש ב-Firebase - כולל אימות קלט
		[RelayCommand]
		private async Task Update()
		{
			//await Toast.Make($"Error deleting user in DB: {ex.Message}", ToastDuration.Short, 14).Show();
			ErrorMessageIsVisible = false;
			if (!Validate())
			{
				//ErrorMessageIsVisible = true;
				await _alertService.ShowAlertAsync("KASATA", ErrorMessage, "OK");
				return;
			}

			AppUser? user = null;

			// If RecievedUser is not null (Came from Admin), use it; otherwise, use the current user
			if (RecievedUser != null)
			{
				user = RecievedUser;
			}
			else
			{
				user = (App.Current as App)!.CurrentUser!;
			}

			IsBusy = true;
			try
			{
				user.FirstName = FirstName;
				user.LastName = LastName;
				user.UserMobile = UserMobile;

				await _dbService.UpdateAsync(user);
				IsBusy = false;

				await _alertService.ShowAlertAsync("KASATA", "User details updated successfully!", "OK");
			}
			catch (Exception ex)
			{
				IsBusy = false;
				await _alertService.ShowAlertAsync("KASATA", $"Error updating user details: {ex.Message}", "OK");
			}
		}

		[RelayCommand]
		private void GetUserImage()
		{
			// Implement get user image functionality here
		}

		// נקודת כניסה - מקבל משתמש מרשימת המשתמשים (מנהל) או טוען את המשתמש הנוכחי
		public void ApplyQueryAttributes(IDictionary<string, object> query)
		{
			RecievedUser = query.ContainsKey("selectedUser") ? (AppUser)query["selectedUser"] : null;

			if (RecievedUser != null) // Load the user from UsersListPage
			{				
				LoadUserDetails(RecievedUser);
				IsDeleteButtonVisible = RecievedUser.Id != (App.Current as App)!.CurrentUser!.Id; // Show delete button if not current user
			}
			else // Load the user from CurrentUser
			{
				// If no user is received, load the current user details
				LoadUserDetails((App.Current as App)!.CurrentUser!);
			}
		}
		// טעינת פרטי המשתמש לשדות ה-UI
		private void LoadUserDetails(AppUser user)
		{
			FirstName = user.FirstName!;
			LastName = user.LastName!;
			UserEmail = user.UserEmail!;
			UserMobile = user.UserMobile!;
			//UserImageBase64 = user.ImageBase64; // Load the user's image base64 string
			UserImageBase64 = null!;
		}

		#region Validation Methods
		private bool Validate()
		{
			var firstNameValid = ValidUserFirstName();
			var lastNameValid = ValidUserLastName();
			var mobileValid = ValidMobile();

			return IsEmptyValidate() && firstNameValid && lastNameValid && mobileValid;
		}
		private bool IsEmptyValidate()
		{
			// Check if any of the required fields are empty
			return !(string.IsNullOrWhiteSpace(FirstName) ||
				   string.IsNullOrWhiteSpace(LastName) ||				   
				   string.IsNullOrWhiteSpace(UserMobile));
		}
		private bool ValidUserFirstName()
		{
			if (FirstName.Length < 2)
			{
				ErrorMessage = "First name too short!";
				return false;
			}
			return true;
		}
		private bool ValidUserLastName()
		{
			if (LastName.Length < 2)
			{
				ErrorMessage = "Last name too short!";				
				return false;
			}
			return true;
		}
		private bool ValidMobile()
		{
			if (UserMobile!.Length != 10)
			{
				ErrorMessage = "Mobile must be between 10 and 15 characters long!";						
				return false;
			}
			return true;
		}
		#endregion

	}
}

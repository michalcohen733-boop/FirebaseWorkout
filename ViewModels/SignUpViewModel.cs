using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using FirebaseWorkout.Helper;
using FirebaseWorkout.Model;
using FirebaseWorkout.Service.DBService;
using FirebaseWorkout.Service.DBService.Firebase;
using FirebaseWorkout.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FirebaseWorkout.ViewModels
{
	// ViewModel של מסך ההרשמה - מנהל יצירת משתמש חדש
	// כולל שדות שם, אימייל, סיסמה, טלפון ואימות קלט
	public partial class SignUpViewModel : ObservableObject
	{
		// אובייקט המשתמש החדש שנוצר בהרשמה
		private AppUser? newUser;
		// שירות משתמשים ליצירת חשבון
		private readonly IAppUserRepository _dbService;
		// ספק שירותים ליצירת Views בצורה Lazy
		private readonly IServiceProvider _services;

		// שדות ההרשמה
		private string _fName;
		private string _lName;
		private string _uEmail;
		private string _uPassword;
		private string _uMobile;

		#region Properties
		// אובייקט ניווט - מוגדר ב-OnAppearing
		public INavigation Navigation { get; set; }
		public string FName
		{
			get => _fName;
			set
			{
				if (_fName != value)
				{
					_fName = value;
					OnPropertyChanged();
					(SignUpCommand as Command).ChangeCanExecute();
				}
			}
		}
		public string LName
		{
			get => _lName;
			set
			{
				if (_lName != value)
				{
					_lName = value; 
					OnPropertyChanged();
					(SignUpCommand as Command).ChangeCanExecute();
				}
			}
		}
		public string UEmail
		{
			get => _uEmail;
			set
			{
				if (_uEmail != value)
				{
					_uEmail = value; 
					OnPropertyChanged();
					(SignUpCommand as Command).ChangeCanExecute();
				}
			}
		}
		public string UPassword
		{
			get => _uPassword;
			set
			{
				if (_uPassword != value)
				{
					_uPassword = value; 
					OnPropertyChanged();
					(SignUpCommand as Command).ChangeCanExecute();
				}
			}
		}
		public string UMobile
		{
			get => _uMobile;
			set
			{
				if (_uMobile != value)
				{
					_uMobile = value; 
					OnPropertyChanged();
					(SignUpCommand as Command).ChangeCanExecute();
				}
			}
		}

		[ObservableProperty]
		private bool _isBusy;

		[ObservableProperty]
		private string _passwordIconCode;

		[ObservableProperty]
		private bool _entryAsPassword;

		[ObservableProperty]
		private bool _signUpMessageVisible;

		[ObservableProperty]
		private string _errorMessage;

		public ICommand SignUpCommand { get; }

		#endregion

		// הקונסטרקטור מקבל IServiceProvider ו-Repository דרך DI
		public SignUpViewModel(IServiceProvider services, IAppUserRepository dbService)
		{
			_services = services;
			_isBusy = false;
			_dbService = dbService;
			_entryAsPassword = true;
			_passwordIconCode = FontHelper.OPEN_EYE_ICON;
			// כפתור SignUp פעיל רק כשכל השדות תקינים (Validate)
			SignUpCommand = new Command(SignUp, Validate);
		}

		// ביצוע הרשמה - יוצר משתמש ב-Firebase ומנווט ל-Shell
		private async void SignUp()
		{
			// הצגת מסך טעינה
			IsBusy = true;

			// בניית אובייקט משתמש חדש מהשדות שהוזנו
			newUser = new AppUser()
			{				
				FirstName = FName,
				LastName = LName,
				UserEmail = UEmail,
				UserPassword = UPassword,
				UserMobile = UMobile,
				RegDate = DateTime.Now.ToShortDateString(),
				UBDate = DateTime.Now.ToShortDateString()
			};

			try
			{
				// שליחת המשתמש ל-Firebase Auth + Database
				newUser.Id = await _dbService!.CreateAsync(newUser);

				//Set as admin
				//await (_dbService as FirebaseUsersRepository)!.SetToAdmin(newUser.Id);
				//newUser.IsAdmin = true;

				IsBusy = false;

				// שמירת המשתמש המחובר באפליקציה
				(App.Current as App)!.CurrentUser = newUser;

				// מעבר ל-Shell (הדף הראשי)
				var mainPage = IPlatformApplication.Current!.Services.GetService<AppShell>();
				Application.Current!.Windows[0].Page = mainPage;
			}
			catch (Exception ex)
			{
				IsBusy = false;
				ShowErrorMessage(ex.Message);
			}
		}

		[RelayCommand]
		private void TogglePassword()
		{
			EntryAsPassword = !EntryAsPassword;
			if (EntryAsPassword)
				PasswordIconCode = FontHelper.OPEN_EYE_ICON;
			else
				PasswordIconCode = FontHelper.CLOSED_EYE_ICON;
		}

		// ניווט למסך ההתחברות - יוצר SignInView דרך DI (Lazy)
		[RelayCommand]
		private async Task NavigateToSignIn()
		{
			try
			{
				if (Navigation == null) return;

				// יצירת SignInView דרך ServiceProvider (מונע Circular Dependency)
				var signInView = _services.GetService<SignInView>();
				if (signInView != null)
				{
					await Navigation.PushAsync(signInView);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(
					$"Navigate to SignIn failed: {ex.Message}");
			}
		}
		// אימות קלט - בודק שכל השדות מלאים ותקינים
		private bool Validate()
		{
			var fnameOK = !string.IsNullOrEmpty(FName);
			var lnameOK = !string.IsNullOrEmpty(LName);
			var emailOK = !string.IsNullOrEmpty(UEmail);
			var passOK = string.IsNullOrEmpty(UPassword) ? false : UPassword.Length > 5;
			var mobileOK = string.IsNullOrEmpty(UMobile) ? false : UMobile.Length == 10;

			return fnameOK && lnameOK && emailOK && passOK && mobileOK;
		}
		private void ShowErrorMessage(string message)
		{
			SignUpMessageVisible = true;
			ErrorMessage = message;
		}
	}
}

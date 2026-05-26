using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using Firebase.Auth.Repository;
using FirebaseWorkout.Helper;
using FirebaseWorkout.Model;
using FirebaseWorkout.Service;
using FirebaseWorkout.Service.DBService;
using FirebaseWorkout.Service.DBService.Firebase;
using FirebaseWorkout.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FirebaseWorkout.ViewModels
{
	// ViewModel של מסך ההתחברות - מנהל כניסה עם אימייל וסיסמה
	// אחרי כניסה מוצלחת מנווט לדף לפי תפקיד: Admin/ServicePerson/רגיל
	public partial class SignInViewModel : ObservableObject
	{
		// ספק שירותים ליצירת Views בצורה Lazy (מונע Circular Dependency)
		private readonly IServiceProvider _services;
		// שירות משתמשים לביצוע כניסה
		private readonly IAppUserRepository _dbService;

		// כתובת אימייל שהמשתמש מקליד
		private string _userEmail;
		// סיסמה שהמשתמש מקליד
		private string _userPassword;

		#region Properties
		// כתובת אימייל - מעדכן את זמינות כפתור SignIn בכל שינוי
		public string UserEmail
		{
			get => _userEmail;
			set
			{
				if (_userEmail != value)
				{
					_userEmail = value;
					OnPropertyChanged();
					(SignInCommand as Command).ChangeCanExecute();

				}
			}
		}
		// סיסמה - מעדכנת את זמינות כפתור SignIn בכל שינוי
		public string UserPassword
		{
			get => _userPassword;
			set
			{
				if (_userPassword != value)
				{
					_userPassword = value;
					OnPropertyChanged();
					(SignInCommand as Command).ChangeCanExecute();
				}
			}
		}

		// אייקון עין לצפייה/הסתרה של הסיסמה
		[ObservableProperty]
		private string _passwordIconCode;

		// האם שדה הסיסמה מוסתר (נקודות)
		[ObservableProperty]
		private bool _entryAsPassword;

		// האם להציג הודעת שגיאה
		[ObservableProperty]
		private bool _signInMessageVisible;

		// האם "זכור אותי" מסומן
		[ObservableProperty]
		private bool _isRememberMeChecked;

		// מצב דיבאג (כבוי בייצור)
		[ObservableProperty]
		private bool _isDebugMode;

		// הודעת שגיאה למשתמש
		[ObservableProperty]
		private string _errorMessage;

		// האם מציג מסך טעינה
		[ObservableProperty]
		private bool _isBusy;

		// אובייקט ניווט - מוגדר ב-OnAppearing של ה-View
		public INavigation Navigation { get; set; }

		#endregion

		// פקודת כניסה - פעילה רק כשאימייל וסיסמה מלאים
		public ICommand SignInCommand { get; }

		// הקונסטרקטור מקבל IServiceProvider (במקום View ישירות) ו-Repository
		public SignInViewModel(IServiceProvider services, IAppUserRepository dbService)
		{
			_services = services;
			_userEmail = string.Empty;
			_userPassword = string.Empty;
			_isBusy = false;
			_dbService = dbService;
			_isDebugMode = false;
			_entryAsPassword = true;
			_passwordIconCode = FontHelper.OPEN_EYE_ICON;
			// כפתור SignIn פעיל רק כשהשדות מלאים
			SignInCommand = new Command(SignIn, () =>
				!(string.IsNullOrEmpty(UserEmail) || string.IsNullOrEmpty(UserPassword)));
		}

		// ביצוע כניסה - מאמת מול Firebase ומנווט לפי תפקיד
		private async void SignIn()
		{
			// הצגת מסך טעינה
			IsBusy = true;
			try
			{
				// אימות מול Firebase Auth + שליפת פרטי המשתמש
				var user = await _dbService.SignInAsync(UserEmail!, UserPassword!);

				IsBusy = false;

				// שמירת המשתמש המחובר באפליקציה
				(App.Current as App)!.CurrentUser = user;

				// מעבר מ-NavigationPage ל-Shell (הדף הראשי של האפליקציה)
				var mainPage = IPlatformApplication.Current!.Services.GetService<AppShell>();
				Application.Current!.Windows[0].Page = mainPage;

				// ניווט לדף ההתחלתי לפי תפקיד המשתמש
				if (user.IsAdmin)
				{
					await Shell.Current.GoToAsync("AdminView");
				}
				else if (user.IsServicePerson)
				{
					await Shell.Current.GoToAsync("ServicePageView");
				}
				// משתמש רגיל נשאר ב-MainPageView (ברירת מחדל של Shell)
			}
			catch (Exception ex)
			{
				IsBusy = false;
				ShowErrorMessage(ex.Message);
			}
		}

		// הצגה/הסתרה של הסיסמה (לחיצה על אייקון העין)
		[RelayCommand]
		private void TogglePassword()
		{
			EntryAsPassword = !EntryAsPassword;
			if (EntryAsPassword)
				PasswordIconCode = FontHelper.OPEN_EYE_ICON;
			else
				PasswordIconCode = FontHelper.CLOSED_EYE_ICON;
		}

		// ניווט למסך ההרשמה - יוצר SignUpView דרך DI (Lazy)
		[RelayCommand]
		private async Task NavigateToSignUp()
		{
			try
			{
				if (Navigation == null) return;

				// יצירת SignUpView דרך ServiceProvider (מונע Circular Dependency)
				var signUpView = _services.GetService<SignUpView>();
				if (signUpView != null)
				{
					await Navigation.PushAsync(signUpView);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(
					$"Navigate to SignUp failed: {ex.Message}");
			}
		}
		// הצגת הודעת שגיאה למשתמש
		private void ShowErrorMessage(string message)
		{
			SignInMessageVisible = true;
			ErrorMessage = message;
		}
	}
}

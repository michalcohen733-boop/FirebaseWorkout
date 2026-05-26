using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FirebaseWorkout.Helper;
using FirebaseWorkout.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FirebaseWorkout.ViewModels
{
	// ViewModel של AppShell - מנהל את סרגל הכלים העליון וכפתורי הניווט
	// קובע אילו כפתורים נראים לפי תפקיד המשתמש (Admin/ServicePerson/Guest/רגיל)
	public partial class AppShellViewModel : ObservableObject
	{
		// דף ההתחברות - לניווט חזרה בעת התנתקות
		private Page _page;

		// האם המשתמש הוא מנהל
		[ObservableProperty]
		public bool? _isAdmin = false;

		// האם המשתמש הוא איש שירות/אב בית
		[ObservableProperty]
		public bool? _isServicePerson = false;

		// האם להציג כפתור Service (מנהל או אב בית)
		[ObservableProperty]
		public bool? _canSeeService = false;

		// האם המשתמש הוא אורח (CurrentUser == null)
		[ObservableProperty]
		public bool? _isGuest = false;

		// האם להציג כפתורי Account ו-Logout (מוסתרים לאורח)
		[ObservableProperty]
		public bool? _showAccountAndLogout = true;

		// אייקונים של הכפתורים בסרגל (Material Icons)
		[ObservableProperty]
		private string _logoutIcon;

		[ObservableProperty]
		private string _adminIcon;

		[ObservableProperty]
		private string _serviceIcon;

		[ObservableProperty]
		private string _homeIcon;

		[ObservableProperty]
		private string _accountIcon;

		// הקונסטרקטור בודק את תפקיד המשתמש וקובע מה להציג
		public AppShellViewModel(SignInView signInView)
		{
			_page = signInView;
			var currentUser = (App.Current as App)?.CurrentUser;
			// בדיקת תפקיד המשתמש
			_isAdmin = currentUser?.IsAdmin ?? false;
			_isServicePerson = currentUser?.IsServicePerson ?? false;
			// Service נראה ל-Admin ול-ServicePerson
			_canSeeService = (_isAdmin ?? false) || (_isServicePerson ?? false);
			// Guest = אין CurrentUser או אין ID
			_isGuest = currentUser == null || string.IsNullOrEmpty(currentUser.Id);
			// אורח לא רואה Account ו-Logout
			_showAccountAndLogout = !(_isGuest ?? true);
			// הגדרת אייקונים
			_logoutIcon = FontHelper.LOGOUT_ICON;
			_adminIcon = FontHelper.ADMIN_ICON;
			_serviceIcon = FontHelper.ADMIN_ICON;
			_homeIcon = FontHelper.HOME_ICON;
			_accountIcon = FontHelper.PERSON_ICON;
		}

		// התנתקות - מנקה את המשתמש וחוזר למסך ההתחברות
		[RelayCommand]
		private void Logout()
		{
			(App.Current as App)!.CurrentUser = null;
			Application.Current.Windows[0].Page = new NavigationPage(_page);
		}

		// ניווט למסך מנהל
		[RelayCommand]
		private async Task NavigateToAdminPage()
		{
			await Shell.Current.GoToAsync("AdminView");
		}

		// ניווט למסך דיווחי שירות
		[RelayCommand]
		private async Task NavigateToServicePage()
		{
			await Shell.Current.GoToAsync("ServicePageView");
		}

		// ניווט למסך הסריקה הראשי
		[RelayCommand]
		private async Task NavigateToHomePage()
		{
			await Shell.Current.GoToAsync("MainPageView");
		}

		// ניווט למסך פרטי חשבון
		[RelayCommand]
		private async Task NavigateToAccountPage()
		{
			await Shell.Current.GoToAsync("AccountView");
		}
	}
}

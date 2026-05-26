using FirebaseWorkout.ViewModels;
using FirebaseWorkout.Views;

namespace FirebaseWorkout
{
    // AppShell - מנהל את הניווט הראשי של האפליקציה (Shell Navigation)
    // מגדיר את כל ה-Routes ומקשר את ה-ViewModel לסרגל הכלים
    public partial class AppShell : Shell
    {
        // הקונסטרקטור מקבל ViewModel דרך DI ומגדיר את כל ה-Routes
        public AppShell(AppShellViewModel vm)
        {
            InitializeComponent();
			BindingContext = vm;
			// רישום כל הדפים לניווט דרך Shell.GoToAsync
			Routing.RegisterRoute(nameof(MainPageView), typeof(MainPageView));
			Routing.RegisterRoute(nameof(AdminView), typeof(AdminView));
			Routing.RegisterRoute(nameof(AccountView), typeof(AccountView));
			Routing.RegisterRoute(nameof(UsersListView), typeof(UsersListView));
			Routing.RegisterRoute(nameof(ReportPageView), typeof(ReportPageView));
			Routing.RegisterRoute(nameof(ServicePageView), typeof(ServicePageView));
			Routing.RegisterRoute(nameof(UpdateUserView), typeof(UpdateUserView));

		}
	}
}

using FirebaseWorkout.ViewModels;
using FirebaseWorkout.Views;

namespace FirebaseWorkout
{
    public partial class AppShell : Shell
    {
        public AppShell(AppShellViewModel vm)
        {
            InitializeComponent();
			BindingContext = vm;
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

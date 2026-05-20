using FirebaseWorkout.Model;
using FirebaseWorkout.Views;

namespace FirebaseWorkout
{
    public partial class App : Application
    {
        private Page _page;
		public AppUser? CurrentUser { get; set; } = null;
        
		public App(HomePageView view)
        {
            InitializeComponent();
            _page = view;
		}

        protected override Window CreateWindow(IActivationState? activationState)
        {
			return new Window(new NavigationPage(_page));
		}
	}
}
using FirebaseWorkout.Model;
using FirebaseWorkout.Views;

namespace FirebaseWorkout
{
    // מחלקת האפליקציה הראשית - מגדירה את הדף הראשון ושומרת את המשתמש המחובר
    public partial class App : Application
    {
        // הדף הראשון שמוצג (HomePageView)
        private Page _page;
		// המשתמש המחובר כרגע - null אם אורח או לא מחובר
		public AppUser? CurrentUser { get; set; } = null;

		// הקונסטרקטור מקבל את HomePageView דרך DI
		public App(HomePageView view)
        {
            InitializeComponent();
            _page = view;
		}

		// יצירת החלון הראשי - עוטף את הדף ב-NavigationPage (לניווט Stack)
        protected override Window CreateWindow(IActivationState? activationState)
        {
			return new Window(new NavigationPage(_page));
		}
	}
}
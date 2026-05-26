using FirebaseWorkout.ViewModels;

namespace FirebaseWorkout.Views;

// Code-behind של מסך הבית הראשי - מגדיר Navigation ו-BindingContext
public partial class HomePageView : ContentPage
{
    // הקונסטרקטור מקבל ViewModel דרך DI ומעביר את ה-Navigation
    public HomePageView(HomePageViewModel vm)
    {
        InitializeComponent();
        // מעביר את אובייקט הניווט ל-ViewModel (לניווט בין דפים)
        vm.Navigation = this.Navigation;
        BindingContext = vm;
    }
}

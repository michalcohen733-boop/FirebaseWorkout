using FirebaseWorkout.ViewModels;

namespace FirebaseWorkout.Views;

public partial class HomePageView : ContentPage
{
    public HomePageView(HomePageViewModel vm)
    {
        InitializeComponent();
        vm.Navigation = this.Navigation;
        BindingContext = vm;
    }
}

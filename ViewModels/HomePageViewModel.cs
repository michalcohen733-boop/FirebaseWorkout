using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FirebaseWorkout.Views;

namespace FirebaseWorkout.ViewModels
{
    public partial class HomePageViewModel : ObservableObject
    {
        private readonly SignInView _signInView;
        private readonly SignUpView _signUpView;

        public INavigation Navigation { get; set; }

        public HomePageViewModel(SignInView signInView, SignUpView signUpView)
        {
            _signInView = signInView;
            _signUpView = signUpView;
        }

        [RelayCommand]
        private async Task SignIn()
        {
            await Navigation!.PushAsync(_signInView);
        }

        [RelayCommand]
        private async Task SignUp()
        {
            await Navigation!.PushAsync(_signUpView);
        }

        [RelayCommand]
        private void EnterAsGuest()
        {
            var appShell = IPlatformApplication.Current!.Services.GetService<AppShell>();
            Application.Current!.Windows[0].Page = appShell;
        }
    }
}

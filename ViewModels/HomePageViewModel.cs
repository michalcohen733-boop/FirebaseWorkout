using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FirebaseWorkout.Views;

namespace FirebaseWorkout.ViewModels
{
    // ViewModel של מסך הבית הראשי - מציג 3 אפשרויות: כניסה, הרשמה, אורח
    // זה המסך הראשון שהמשתמש רואה כשפותח את האפליקציה
    public partial class HomePageViewModel : ObservableObject
    {
        // דף ההתחברות (מוזרק דרך DI)
        private readonly SignInView _signInView;
        // דף ההרשמה (מוזרק דרך DI)
        private readonly SignUpView _signUpView;

        // אובייקט ניווט - מוגדר ב-code-behind של ה-View
        public INavigation Navigation { get; set; }

        // הקונסטרקטור מקבל את דפי ההתחברות וההרשמה דרך DI
        public HomePageViewModel(SignInView signInView, SignUpView signUpView)
        {
            _signInView = signInView;
            _signUpView = signUpView;
        }

        // ניווט למסך ההתחברות
        [RelayCommand]
        private async Task SignIn()
        {
            await Navigation!.PushAsync(_signInView);
        }

        // ניווט למסך ההרשמה
        [RelayCommand]
        private async Task SignUp()
        {
            await Navigation!.PushAsync(_signUpView);
        }

        // כניסה כאורח - CurrentUser נשאר null
        // עובר ישירות ל-AppShell עם מסך הסריקה
        [RelayCommand]
        private void EnterAsGuest()
        {
            var appShell = IPlatformApplication.Current!.Services.GetService<AppShell>();
            Application.Current!.Windows[0].Page = appShell;
        }
    }
}

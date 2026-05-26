using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FirebaseWorkout.ViewModels
{
    // ViewModel של מסך מנהל - מציג 2 אפשרויות: ניהול משתמשים ודיווחי שירות
    public partial class AdminViewModel : ObservableObject
    {
        public AdminViewModel()
        {
        }

        // ניווט למסך רשימת המשתמשים
        [RelayCommand]
        private async Task NavigateToUsersListView()
        {
            await Shell.Current.GoToAsync("UsersListView");
        }

        // ניווט למסך דיווחי שירות (דיווחים פתוחים)
        [RelayCommand]
        private async Task Service()
        {
            await Shell.Current.GoToAsync("ServicePageView");
        }
    }
}

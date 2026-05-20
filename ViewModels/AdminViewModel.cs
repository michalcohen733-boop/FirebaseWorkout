using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FirebaseWorkout.ViewModels
{
    public partial class AdminViewModel : ObservableObject
    {
        public AdminViewModel()
        {
        }

        [RelayCommand]
        private async Task NavigateToUsersListView()
        {
            await Shell.Current.GoToAsync("UsersListView");
        }

        [RelayCommand]
        private async Task Service()
        {
            await Shell.Current.GoToAsync("ServicePageView");
        }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FirebaseWorkout.Model;
using FirebaseWorkout.Service;
using FirebaseWorkout.Service.DBService;
using Microsoft.Maui.Graphics;

namespace FirebaseWorkout.ViewModels
{
    public partial class UpdateUserViewModel : ObservableObject, IQueryAttributable
    {
        private readonly IAppUserRepository _userRepo;
        private readonly IAlertService _alertService;
        private readonly IAppLogger _appLogger;

        [ObservableProperty]
        private AppUser _user;

        [ObservableProperty]
        private string _statusMessage;

        [ObservableProperty]
        private bool _hasStatusMessage;

        [ObservableProperty]
        private Color _statusColor = Color.FromArgb("#EF4444");

        public UpdateUserViewModel(
            IAppUserRepository userRepo,
            IAlertService alertService,
            IAppLogger appLogger)
        {
            _userRepo = userRepo;
            _alertService = alertService;
            _appLogger = appLogger;
            User = new AppUser();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("selectedUser", out var obj) && obj is AppUser user)
                User = user;
        }

        [RelayCommand]
        private async Task Update()
        {
            if (string.IsNullOrWhiteSpace(User.FirstName) ||
                string.IsNullOrWhiteSpace(User.LastName))
            {
                StatusMessage = "Name fields cannot be empty.";
                StatusColor = Color.FromArgb("#EF4444");
                HasStatusMessage = true;
                return;
            }

            if (!string.IsNullOrEmpty(User.UserMobile) && User.UserMobile.Length != 10)
            {
                StatusMessage = "Phone must be 10 digits.";
                StatusColor = Color.FromArgb("#EF4444");
                HasStatusMessage = true;
                return;
            }

            try
            {
                await _userRepo.UpdateAsync(User);
                StatusMessage = "User updated successfully!";
                StatusColor = Color.FromArgb("#10B981");
                HasStatusMessage = true;

                await Task.Delay(1500);
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                _appLogger.LogDebug($"Update failed: {ex.Message}");
                StatusMessage = "Failed to update user.";
                StatusColor = Color.FromArgb("#EF4444");
                HasStatusMessage = true;
            }
        }
    }
}

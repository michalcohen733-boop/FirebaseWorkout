using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FirebaseWorkout.Model;
using FirebaseWorkout.Service;
using FirebaseWorkout.Service.DBService;
using Microsoft.Maui.Graphics;

namespace FirebaseWorkout.ViewModels
{
    // ViewModel של מסך עריכת משתמש (מנהל) - מאפשר עדכון פרטי משתמש
    // מממש IQueryAttributable כדי לקבל את המשתמש הנבחר מרשימת המשתמשים
    public partial class UpdateUserViewModel : ObservableObject, IQueryAttributable
    {
        // Repository משתמשים
        private readonly IAppUserRepository _userRepo;
        // שירות התראות
        private readonly IAlertService _alertService;
        // שירות לוגים
        private readonly IAppLogger _appLogger;

        // המשתמש שנערך
        [ObservableProperty]
        private AppUser _user;

        // הודעת סטטוס (שגיאה או הצלחה)
        [ObservableProperty]
        private string _statusMessage;

        // האם להציג הודעת סטטוס
        [ObservableProperty]
        private bool _hasStatusMessage;

        // צבע הודעת הסטטוס (אדום=שגיאה, ירוק=הצלחה)
        [ObservableProperty]
        private Color _statusColor = Color.FromArgb("#EF4444");

        // הקונסטרקטור מקבל שירותים דרך DI
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

        // מקבל את המשתמש הנבחר שהועבר מרשימת המשתמשים
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("selectedUser", out var obj) && obj is AppUser user)
                User = user;
        }

        // עדכון פרטי המשתמש ב-Firebase - כולל אימות קלט
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

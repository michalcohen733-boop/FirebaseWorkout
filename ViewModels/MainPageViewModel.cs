using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FirebaseWorkout.Model;
using FirebaseWorkout.Service.DBService;

namespace FirebaseWorkout.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly IComputerRepository _computerRepo;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _manualCode;

        [ObservableProperty]
        private bool _isScanning = true;

        [ObservableProperty]
        private string _statusMessage;

        [ObservableProperty]
        private bool _hasStatusMessage;

        partial void OnStatusMessageChanged(string value)
        {
            HasStatusMessage = !string.IsNullOrEmpty(value);
        }

        public MainPageViewModel(IComputerRepository computerRepo)
        {
            _computerRepo = computerRepo;
            var currentUser = (App.Current as App)?.CurrentUser;
            _name = currentUser != null ? "Hello " + currentUser.FirstName : "Hello Guest";
        }

        [RelayCommand]
        private async Task Settings()
        {
            await Shell.Current.GoToAsync("AccountView");
        }

        [RelayCommand]
        private async Task SubmitManualCode()
        {
            await HandleScannedCodeAsync(ManualCode);
        }

        public async Task HandleScannedCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                StatusMessage = "Please enter a valid code.";
                return;
            }

            IsScanning = false;

            try
            {
                Computer? computer = await _computerRepo.GetByQRCodeAsync(code);

                if (computer == null)
                {
                    StatusMessage = "Computer not found. Please check the code.";
                    await Task.Delay(1000);
                    IsScanning = true;
                    return;
                }

                StatusMessage = string.Empty;
                ManualCode = string.Empty;

                await Shell.Current.GoToAsync("ReportPageView",
                    new Dictionary<string, object> { { "computer", computer } });

                IsScanning = true;
            }
            catch (Exception)
            {
                StatusMessage = "Connection error. Please try again.";
                await Task.Delay(1000);
                IsScanning = true;
            }
        }
    }
}

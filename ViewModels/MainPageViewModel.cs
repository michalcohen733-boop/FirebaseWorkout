using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FirebaseWorkout.Model;
using FirebaseWorkout.Service.DBService;

namespace FirebaseWorkout.ViewModels
{
    // ViewModel של מסך הסריקה הראשי - סורק QR Code או מקבל קוד ידני
    // מחפש את המחשב ב-Firebase ומנווט למסך דיווח תקלה
    public partial class MainPageViewModel : ObservableObject
    {
        // Repository מחשבים לחיפוש לפי QR Code
        private readonly IComputerRepository _computerRepo;

        // שם המשתמש המחובר (או "Guest")
        [ObservableProperty]
        private string _name;

        // קוד שהוזן ידנית בשדה הטקסט
        [ObservableProperty]
        private string _manualCode;

        // האם המצלמה סורקת כרגע
        [ObservableProperty]
        private bool _isScanning = true;

        // הודעת סטטוס/שגיאה למשתמש
        [ObservableProperty]
        private string _statusMessage;

        // האם יש הודעת סטטוס להציג
        [ObservableProperty]
        private bool _hasStatusMessage;

        // מתעדכן אוטומטית כשהודעת הסטטוס משתנה
        partial void OnStatusMessageChanged(string value)
        {
            HasStatusMessage = !string.IsNullOrEmpty(value);
        }

        // הקונסטרקטור מקבל Repository דרך DI ומציג ברכה למשתמש
        public MainPageViewModel(IComputerRepository computerRepo)
        {
            _computerRepo = computerRepo;
            var currentUser = (App.Current as App)?.CurrentUser;
            _name = currentUser != null ? "Hello " + currentUser.FirstName : "Hello Guest";
        }

        // ניווט למסך הגדרות חשבון
        [RelayCommand]
        private async Task Settings()
        {
            await Shell.Current.GoToAsync("AccountView");
        }

        // שליחת קוד שהוזן ידנית - קורא ל-HandleScannedCodeAsync
        [RelayCommand]
        private async Task SubmitManualCode()
        {
            await HandleScannedCodeAsync(ManualCode);
        }

        // מטפל בקוד שנסרק או הוזן ידנית
        // מחפש מחשב ב-Firebase ומנווט למסך דיווח אם נמצא
        public async Task HandleScannedCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                StatusMessage = "Please enter a valid code.";
                return;
            }

            // עצירת הסריקה בזמן חיפוש
            IsScanning = false;

            try
            {
                // חיפוש המחשב ב-Firebase לפי קוד QR
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

                // ניווט למסך דיווח תקלה עם העברת אובייקט המחשב
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

using BarcodeScanning;
using FirebaseWorkout.ViewModels;
using Microsoft.Maui.ApplicationModel;

namespace FirebaseWorkout.Views;

// Code-behind של מסך הסריקה - מנהל את המצלמה וזיהוי ברקודים
public partial class MainPageView : ContentPage
{
    // שמירת ה-ViewModel לגישה ישירה (לסריקת QR)
    private readonly MainPageViewModel _vm;

    // הקונסטרקטור מקבל ViewModel דרך DI
    public MainPageView(MainPageViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    // כשהמסך מופיע - מציג את אייקון המצלמה (לא פותח מצלמה אוטומטית)
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.IsPlaceholderVisible = true;
        _vm.IsCameraVisible = false;
        _vm.IsScanning = false;
    }

    // כשיוצאים מהמסך - כיבוי המצלמה ואיפוס המצב
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _vm.IsCameraVisible = false;
        _vm.IsPlaceholderVisible = true;
        _vm.IsScanning = false;
    }

    // Event Handler לזיהוי ברקוד - נקרא כשהמצלמה מזהה QR Code
    private void OnBarcodesDetected(object sender, OnDetectionFinishedEventArg e)
    {
        // ביצוע ב-Main Thread (נדרש לעדכון UI)
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (e.BarcodeResults?.Length > 0)
            {
                // כיבוי המצלמה וחזרה לאייקון
                _vm.IsCameraVisible = false;
                _vm.IsPlaceholderVisible = true;
                _vm.IsScanning = false;

                // שליחת הקוד שנסרק ל-ViewModel לחיפוש ב-Firebase
                string scannedCode = e.BarcodeResults[0].DisplayValue;
                await _vm.HandleScannedCodeAsync(scannedCode);
            }
        });
    }
}

using BarcodeScanning;
using FirebaseWorkout.ViewModels;
using Microsoft.Maui.ApplicationModel;

namespace FirebaseWorkout.Views;

// Code-behind של מסך הסריקה - מנהל את המצלמה והרשאות
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

    // כשהמסך מופיע - בקשת הרשאת מצלמה והפעלתה
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // בדיקה ובקשת הרשאת מצלמה
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
            status = await Permissions.RequestAsync<Permissions.Camera>();

        if (status != PermissionStatus.Granted)
        {
            await DisplayAlert("Camera Permission",
                "Camera permission denied. You can still enter codes manually.", "OK");
            return;
        }

        // הפעלת המצלמה לסריקה
        cameraView.CameraEnabled = true;
    }

    // כשיוצאים מהמסך - כיבוי המצלמה
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        cameraView.CameraEnabled = false;
    }

    // Event Handler לזיהוי ברקוד - נקרא כשהמצלמה מזהה QR Code
    private void OnBarcodesDetected(object sender, OnDetectionFinishedEventArg e)
    {
        // ביצוע ב-Main Thread (נדרש לעדכון UI)
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (e.BarcodeResults?.Length > 0)
            {
                // עצירת המצלמה בזמן עיבוד
                cameraView.CameraEnabled = false;
                // שליחת הקוד שנסרק ל-ViewModel לחיפוש ב-Firebase
                string scannedCode = e.BarcodeResults[0].DisplayValue;
                await _vm.HandleScannedCodeAsync(scannedCode);
                // הפעלת המצלמה מחדש
                cameraView.CameraEnabled = true;
            }
        });
    }
}

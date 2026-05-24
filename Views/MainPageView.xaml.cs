using BarcodeScanning;
using FirebaseWorkout.ViewModels;
using Microsoft.Maui.ApplicationModel;

namespace FirebaseWorkout.Views;

public partial class MainPageView : ContentPage
{
    private readonly MainPageViewModel _vm;

    public MainPageView(MainPageViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
            status = await Permissions.RequestAsync<Permissions.Camera>();

        if (status != PermissionStatus.Granted)
        {
            await DisplayAlert("Camera Permission",
                "Camera permission denied. You can still enter codes manually.", "OK");
            return;
        }

        cameraView.CameraEnabled = true;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        cameraView.CameraEnabled = false;
    }

    private void OnBarcodesDetected(object sender, OnDetectionFinishedEventArg e)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (e.BarcodeResults?.Length > 0)
            {
                cameraView.CameraEnabled = false;
                string scannedCode = e.BarcodeResults[0].DisplayValue;
                await _vm.HandleScannedCodeAsync(scannedCode);
                cameraView.CameraEnabled = true;
            }
        });
    }
}

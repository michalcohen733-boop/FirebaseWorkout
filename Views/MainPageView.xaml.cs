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

        await Task.Delay(500);
        _vm.IsScanning = false;
        await Task.Delay(100);
        _vm.IsScanning = true;
    }

    private void OnBarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (e.Results?.Length > 0)
            {
                string scannedCode = e.Results[0].Value;
                await _vm.HandleScannedCodeAsync(scannedCode);
            }
        });
    }
}

using FirebaseWorkout.Model;
using FirebaseWorkout.ViewModels;

namespace FirebaseWorkout.Views;

public partial class ServicePageView : ContentPage
{
    private readonly ServicePageViewModel _vm;

    public ServicePageView(ServicePageViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadReportsAsync();
        _vm.SubscribeToChanges();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _vm.UnsubscribeFromChanges();
    }

    private async void OnReportSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Report selectedReport)
        {
            await _vm.HandleReportSelectedAsync(selectedReport);
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}

using FirebaseWorkout.Model;
using FirebaseWorkout.ViewModels;

namespace FirebaseWorkout.Views;

// Code-behind של מסך שירות - מנהל מחזור חיים של עדכונים בזמן אמת
public partial class ServicePageView : ContentPage
{
    // שמירת ה-ViewModel לגישה ישירה
    private readonly ServicePageViewModel _vm;

    // הקונסטרקטור מקבל ViewModel דרך DI
    public ServicePageView(ServicePageViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    // כשהמסך מופיע - טוען דיווחים ונרשם לעדכונים בזמן אמת
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadReportsAsync();
        _vm.SubscribeToChanges();
    }

    // כשיוצאים מהמסך - מבטל הרשמה לעדכונים (חיסכון במשאבים)
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _vm.UnsubscribeFromChanges();
    }

    // Event Handler ללחיצה על דיווח ברשימה - מעביר ל-ViewModel לטיפול
    private async void OnReportSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Report selectedReport)
        {
            await _vm.HandleReportSelectedAsync(selectedReport);
            // איפוס הבחירה כדי שאפשר יהיה ללחוץ שוב על אותו פריט
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}

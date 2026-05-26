using FirebaseWorkout.ViewModels;

namespace FirebaseWorkout.Views;

// Code-behind של מסך דיווח תקלה
public partial class ReportPageView : ContentPage
{
    // הקונסטרקטור מקבל ViewModel דרך DI ומגדיר אותו כ-BindingContext
    public ReportPageView(ReportPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}

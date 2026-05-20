using FirebaseWorkout.ViewModels;

namespace FirebaseWorkout.Views;

public partial class ReportPageView : ContentPage
{
    public ReportPageView(ReportPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}

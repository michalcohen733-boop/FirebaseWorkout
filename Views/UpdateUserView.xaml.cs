using FirebaseWorkout.ViewModels;

namespace FirebaseWorkout.Views;

public partial class UpdateUserView : ContentPage
{
    public UpdateUserView(UpdateUserViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}

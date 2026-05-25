using FirebaseWorkout.ViewModels;

namespace FirebaseWorkout.Views;

public partial class SignUpView : ContentPage
{
	public SignUpView(SignUpViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is SignUpViewModel vm)
			vm.Navigation = this.Navigation;
	}
}
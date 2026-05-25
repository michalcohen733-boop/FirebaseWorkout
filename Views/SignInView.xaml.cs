using FirebaseWorkout.ViewModels;

namespace FirebaseWorkout.Views;

public partial class SignInView : ContentPage
{
	public SignInView(SignInViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is SignInViewModel vm)
			vm.Navigation = this.Navigation;
	}
}
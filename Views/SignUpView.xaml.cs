using FirebaseWorkout.ViewModels;

namespace FirebaseWorkout.Views;

// Code-behind של מסך ההרשמה
public partial class SignUpView : ContentPage
{
	// הקונסטרקטור מקבל ViewModel דרך DI
	public SignUpView(SignUpViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

	// מעביר את ה-Navigation ל-ViewModel כשהמסך מופיע
	protected override void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is SignUpViewModel vm)
			vm.Navigation = this.Navigation;
	}
}
using FirebaseWorkout.ViewModels;

namespace FirebaseWorkout.Views;

// Code-behind של מסך ההתחברות
public partial class SignInView : ContentPage
{
	// הקונסטרקטור מקבל ViewModel דרך DI
	public SignInView(SignInViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

	// מעביר את ה-Navigation ל-ViewModel כשהמסך מופיע
	// (ב-OnAppearing ולא בקונסטרקטור כי הניווט עדיין לא מחובר בקונסטרקטור)
	protected override void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is SignInViewModel vm)
			vm.Navigation = this.Navigation;
	}
}
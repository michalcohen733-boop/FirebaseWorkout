using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using Firebase.Auth.Repository;
using FirebaseWorkout.Helper;
using FirebaseWorkout.Model;
using FirebaseWorkout.Service;
using FirebaseWorkout.Service.DBService;
using FirebaseWorkout.Service.DBService.Firebase;
using FirebaseWorkout.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FirebaseWorkout.ViewModels
{
	public partial class SignInViewModel : ObservableObject
	{
		private readonly IServiceProvider _services;
		private readonly IAppUserRepository _dbService;

		private string _userEmail;
		private string _userPassword;

		#region Properties
		public string UserEmail
		{
			get => _userEmail;
			set
			{
				if (_userEmail != value)
				{
					_userEmail = value;
					OnPropertyChanged();
					(SignInCommand as Command).ChangeCanExecute();

				}
			}
		}
		public string UserPassword
		{
			get => _userPassword;
			set
			{
				if (_userPassword != value)
				{
					_userPassword = value;
					OnPropertyChanged();
					(SignInCommand as Command).ChangeCanExecute();
				}
			}
		}	

		[ObservableProperty]
		private string _passwordIconCode;

		[ObservableProperty]
		private bool _entryAsPassword;

		[ObservableProperty]
		private bool _signInMessageVisible;

		[ObservableProperty]
		private bool _isRememberMeChecked;

		[ObservableProperty]
		private bool _isDebugMode;

		[ObservableProperty]
		private string _errorMessage;

		[ObservableProperty]
		private bool _isBusy;

		public INavigation Navigation { get; set; }

		//public string Name => "Wellcome " + _authClient.User?.Info?.DisplayName!;		
		#endregion

		public ICommand SignInCommand { get; }

		public SignInViewModel(IServiceProvider services, IAppUserRepository dbService)
		{
			_services = services;
			_userEmail = string.Empty;
			_userPassword = string.Empty;
			_isBusy = false;
			_dbService = dbService;
			_isDebugMode = false;
			_entryAsPassword = true;
			_passwordIconCode = FontHelper.OPEN_EYE_ICON;
			SignInCommand = new Command(SignIn, () =>
				!(string.IsNullOrEmpty(UserEmail) || string.IsNullOrEmpty(UserPassword)));						
		}

		private async void SignIn()
		{
			//Show Progress Bar
			IsBusy = true;
			try
			{
				var user = await _dbService.SignInAsync(UserEmail!, UserPassword!);
				
				IsBusy = false;

				//Set CurrentUser
				(App.Current as App)!.CurrentUser = user;

				// Navigate to Shell
				var mainPage = IPlatformApplication.Current!.Services.GetService<AppShell>();
				Application.Current!.Windows[0].Page = mainPage;

				// Navigate to role-based start page
				if (user.IsAdmin)
				{
					await Shell.Current.GoToAsync("AdminView");
				}
				else if (user.IsServicePerson)
				{
					await Shell.Current.GoToAsync("ServicePageView");
				}		
			}
			catch (Exception ex)
			{
				IsBusy = false;
				ShowErrorMessage(ex.Message);
			}
		}

		[RelayCommand]
		private void TogglePassword()
		{
			EntryAsPassword = !EntryAsPassword;
			if (EntryAsPassword)
				PasswordIconCode = FontHelper.OPEN_EYE_ICON;
			else
				PasswordIconCode = FontHelper.CLOSED_EYE_ICON;
		}

		[RelayCommand]
		private async Task NavigateToSignUp()
		{
			try
			{
				if (Navigation == null) return;

				var signUpView = _services.GetService<SignUpView>();
				if (signUpView != null)
				{
					await Navigation.PushAsync(signUpView);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(
					$"Navigate to SignUp failed: {ex.Message}");
			}
		}		
		private void ShowErrorMessage(string message)
		{
			SignInMessageVisible = true;
			ErrorMessage = message;
		}
	}
}

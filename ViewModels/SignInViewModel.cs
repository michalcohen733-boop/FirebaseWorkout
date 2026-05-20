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
		private readonly Page _page;
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

		public SignInViewModel(SignUpView view, IAppUserRepository dbService)
		{
			//Debug Mode

			_userEmail = "konstant_z@yahoo.com";
			_userPassword = "123456";
			_page = view;
			_isBusy = false;
			_dbService = dbService;			
			_isDebugMode = true;
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

				// Navigate to Main Page of Shell
				var mainPage = IPlatformApplication.Current!.Services.GetService<AppShell>();
				Application.Current!.Windows[0].Page = mainPage;		
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
				await Navigation!.PushAsync(_page);
			}
			catch (Exception ex)
			{
				var message = ex.Message;
			}
		}		
		private void ShowErrorMessage(string message)
		{
			SignInMessageVisible = true;
			ErrorMessage = message;
		}
	}
}

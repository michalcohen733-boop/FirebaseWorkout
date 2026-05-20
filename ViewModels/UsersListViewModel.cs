using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using Firebase.Database.Streaming;
using FirebaseWorkout.Helper;
using FirebaseWorkout.Model;
using FirebaseWorkout.Service;
using FirebaseWorkout.Service.DBService;
using FirebaseWorkout.Service.DBService.Firebase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FirebaseWorkout.ViewModels
{
	public partial class UsersListViewModel : ObservableObject
	{
		private readonly IAppLogger _appLogger;
		private readonly IAlertService _alertService;
		private readonly IAppUserRepository _dbService;

		IDisposable? _dbSubscription; // Cancel subscription to db updates when not needed
		private List<AppUser> _allUsers = new(); //List of users to be displayed
		public ObservableCollection<AppUser> AllUsers { get; set; }

		[ObservableProperty]
		private AppUser? _selectedUser;

		[ObservableProperty]
		private bool _isBusy;

		[ObservableProperty]
		private string _filterIcon;

		[ObservableProperty]
		private string _searchText;

		//public Command? GetAllUsersCommand { get { return new Command(GetUsersListFromDB); } }

		public UsersListViewModel(IAlertService alertService, IAppUserRepository dbService, IAppLogger appLogger)
		{
			_appLogger = appLogger;
			_alertService = alertService;
			_dbService = dbService;		
			FilterIcon = FontHelper.FILTER_ON_ICON;
			AllUsers = new ObservableCollection<AppUser>();
		}

		///////////////////////////////////////////////////////////////////

		[RelayCommand]
		private void ClearFilter()
		{
			throw new NotImplementedException();
		}

		[RelayCommand]
		private void Search()
		{
			throw new NotImplementedException();
		}

		[RelayCommand]
		private async Task NavigateToAccountPage()
		{
			if (SelectedUser != null)
			{
				Dictionary<string, object> param = new Dictionary<string, object>();
				param.Add("selectedUser", SelectedUser);
				await Shell.Current.GoToAsync("UpdateUserView", param);
			}
		}

		private async Task SubscribeToDbUpdates()
		{
			if (_dbSubscription != null) CancelDbSubscription();

			_dbSubscription = (_dbService as FirebaseUsersRepository)!.SubscribeToUserChanges()
				.Subscribe(item =>
				{
					//Update UI on main thread
					//Shell.Current.Dispatcher.Dispatch(() =>
					MainThread.BeginInvokeOnMainThread(() =>
					{
						if (item.EventType == FirebaseEventType.InsertOrUpdate)
						{
							AddOrUpdateUser(item.Object);
						}
						else if (item.EventType == FirebaseEventType.Delete)
						{
							// משתמשים ב-Key למחיקה בטוחה
							RemoveUser(item.Key);
						}
						FillUsersList();
					});
				},
				ex => _appLogger.LogDebug($"Error: {ex.Message}"));
		}

		private void FillUsersList()
		{
			AllUsers.Clear(); // Clear the existing collection
			foreach (var user in _allUsers)
			{
				AllUsers.Add(user); // Add each user to the ObservableCollection
			}			
		}

		private void AddOrUpdateUser(AppUser item)
		{
			//Check if user already exists in the list
			var index = _allUsers.FindIndex(u => u.Id == item.Id);

			if (index != -1) // המשתמש קיים - נחליף אותו במיקום שלו
			{
				_allUsers[index] = item;
			}
			else // משתמש חדש - נוסיף לרשימה
			{
				_allUsers.Add(item);
			}
		}

		private void RemoveUser(string userId)
		{
			//bool confirm = await Shell.Current.DisplayAlert("Firebase App", "Remove User?", "Yes","No");
			var item = _allUsers.Where(u => u.Id == userId).FirstOrDefault();
			if (item != null)
			{
				_allUsers.Remove(item);
			}
		}
		
		private void CancelDbSubscription()
		{
			_dbSubscription?.Dispose();
			_dbSubscription = null;
		}

		internal async void OnAppearing()
		{
			//Clear existing users list before subscribing to db updates
			_allUsers.Clear();
			await SubscribeToDbUpdates(); //Subscribe to db updates			
			SelectedUser = null!;
		}

		internal void OnDisappearing()
		{
			CancelDbSubscription();
		}
	}
}

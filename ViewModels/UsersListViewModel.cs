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
	// ViewModel של רשימת משתמשים (מסך מנהל) - מציג כל המשתמשים בזמן אמת
	// מאזין לשינויים ב-Firebase ומעדכן אוטומטית
	public partial class UsersListViewModel : ObservableObject
	{
		// שירות לוגים
		private readonly IAppLogger _appLogger;
		// שירות התראות
		private readonly IAlertService _alertService;
		// Repository משתמשים
		private readonly IAppUserRepository _dbService;

		// מנוי לעדכונים בזמן אמת מ-Firebase
		IDisposable? _dbSubscription;
		// רשימת משתמשים פנימית (מקור נתונים)
		private List<AppUser> _allUsers = new();
		// רשימה שמוצגת ב-UI (ObservableCollection מעדכנת את ה-View)
		public ObservableCollection<AppUser> AllUsers { get; set; }

		// המשתמש שנבחר ברשימה
		[ObservableProperty]
		private AppUser? _selectedUser;

		// האם מציג מסך טעינה
		[ObservableProperty]
		private bool _isBusy;

		// אייקון סינון
		[ObservableProperty]
		private string _filterIcon;

		// טקסט חיפוש
		[ObservableProperty]
		private string _searchText;

		//public Command? GetAllUsersCommand { get { return new Command(GetUsersListFromDB); } }

		// הקונסטרקטור מקבל שירותים דרך DI
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

		// ניווט למסך עריכת משתמש - מעביר את המשתמש הנבחר
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

		// הרשמה לעדכונים בזמן אמת מ-Firebase - מעדכן רשימה כשמשתמש נוסף/נערך/נמחק
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

		// מעדכן את ה-ObservableCollection מהרשימה הפנימית
		private void FillUsersList()
		{
			AllUsers.Clear(); // Clear the existing collection
			foreach (var user in _allUsers)
			{
				AllUsers.Add(user); // Add each user to the ObservableCollection
			}			
		}

		// הוספה או עדכון של משתמש ברשימה הפנימית
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

		// הסרת משתמש מהרשימה הפנימית לפי ID
		private void RemoveUser(string userId)
		{
			//bool confirm = await Shell.Current.DisplayAlert("Firebase App", "Remove User?", "Yes","No");
			var item = _allUsers.Where(u => u.Id == userId).FirstOrDefault();
			if (item != null)
			{
				_allUsers.Remove(item);
			}
		}
		
		// ביטול ההרשמה לעדכוני Firebase
		private void CancelDbSubscription()
		{
			_dbSubscription?.Dispose();
			_dbSubscription = null;
		}

		// נקרא כשהמסך מופיע - מנקה ונרשם לעדכונים
		internal async void OnAppearing()
		{
			//Clear existing users list before subscribing to db updates
			_allUsers.Clear();
			await SubscribeToDbUpdates(); //Subscribe to db updates			
			SelectedUser = null!;
		}

		// נקרא כשיוצאים מהמסך - מבטל הרשמה לעדכונים
		internal void OnDisappearing()
		{
			CancelDbSubscription();
		}
	}
}

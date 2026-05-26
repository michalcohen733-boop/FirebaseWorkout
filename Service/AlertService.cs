using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseWorkout.Service
{
	// שירות התראות - מציג חלון הודעה למשתמש דרך Shell
	public class AlertService : IAlertService
	{
		// מציג Alert Dialog באמצעות Shell.Current
		public Task ShowAlertAsync(string title, string message, string cancel)
		{
			return Shell.Current.DisplayAlert(title, message, cancel);
		}
	}
}

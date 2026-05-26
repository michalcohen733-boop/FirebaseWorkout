using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseWorkout.Service
{
	// ממשק שירות התראות - להצגת הודעות למשתמש
	public interface IAlertService
	{
		// הצגת חלון התראה עם כותרת, הודעה וכפתור ביטול
		Task ShowAlertAsync(string title, string message, string cancel);
	}
}

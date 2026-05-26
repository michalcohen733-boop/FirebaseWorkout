using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseWorkout.Service
{
	// שירות לוגים - כותב הודעות דיבאג ושגיאות ל-Android Log
	public class LogService : IAppLogger
	{
		// תג לזיהוי ההודעות ב-Logcat
		private readonly string TAG = "KASATA";

		// כתיבת הודעת דיבאג ל-Android Logcat
		public void LogDebug(string message)
		{
#if ANDROID
			Android.Util.Log.Debug(TAG,message);
#endif
			//Debug.WriteLine($"Log {TAG}: {message}");

		}

		// כתיבת הודעת שגיאה ל-Android Logcat
		public void LogError(string message)
		{
#if ANDROID
			Android.Util.Log.Error(TAG,message);
#endif
			//Debug.WriteLine($"Error {TAG}: {message}");
		}
	}
}

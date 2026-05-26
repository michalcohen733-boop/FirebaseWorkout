using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseWorkout.Service.DBService.Firebase
{
	// מחלקת בסיס לכל ה-Repositories שעובדים מול Firebase
	// יוצרת את החיבור ל-Realtime Database ומעבירה אותו לכל היורשים
	public class FirebaseRealtimeService : IDbInstance
	{
		// לקוח Firebase - משותף לכל ה-Repositories היורשים
		protected FirebaseClient? _firebaseClient;

		// הקונסטרקטור יוצר חיבור ל-Firebase Realtime Database
		public FirebaseRealtimeService()
		{
			_firebaseClient = new FirebaseClient("https://qrcodedb-19df3-default-rtdb.firebaseio.com/");
		}
		// מחזיר מידע על סוג המסד (לצורך דיבאג)
		public string Info()
		{
			return "Type: Google Firebase RealTime Database client";
		}
	}
}

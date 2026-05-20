using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseWorkout.Service.DBService.Firebase
{
	public class FirebaseRealtimeService : IDbInstance
	{
		protected FirebaseClient? _firebaseClient;

		public FirebaseRealtimeService()
		{
			_firebaseClient = new FirebaseClient("https://qrcodedb-19df3-default-rtdb.firebaseio.com/");
		}
		public string Info()
		{
			return "Type: Google Firebase RealTime Database client";
		}
	}
}

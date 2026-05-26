using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseWorkout.Service.DBService.Firebase
{
	// ממשק שירות אימות - מגדיר פעולות כניסה, הרשמה ומחיקה של משתמשים
	public interface IAuthService
	{
		// כניסה עם אימייל וסיסמה, מחזיר את ה-User ID
		Task<string> SignIn(string usreEmail, string userPassword);
		// יצירת משתמש חדש ב-Auth, מחזיר את ה-User ID
		Task<string> CreateAuth(string email, string password);
		// מחיקת משתמש מ-Auth
		Task RemoveAuth(string email, string password);
		// יציאה מהמערכת
		Task SignOut();
	}
}

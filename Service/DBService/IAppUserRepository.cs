using FirebaseWorkout.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseWorkout.Service.DBService
{
	// ממשק Repository של משתמשים - מגדיר פעולות CRUD על משתמשים
	public interface IAppUserRepository
	{
		// יצירת משתמש חדש, מחזיר את ה-ID
		Task<string> CreateAsync(AppUser appUser);
		// עדכון פרטי משתמש קיים
		Task UpdateAsync(AppUser appUser);
		// מחיקת משתמש (גם מ-Auth וגם מ-Database)
		Task DeleteAsync(AppUser appUser);
		// כניסה עם אימייל וסיסמה, מחזיר את אובייקט המשתמש
		Task<AppUser> SignInAsync(string userEmail, string userPassword);
		// שליפת משתמש לפי מזהה
		Task<AppUser> GetUserByIdAsync(string userId);
		// שליפת כל המשתמשים
		List<AppUser> GetAllAsync();
		// הפיכת משתמש למנהל
		Task SetToAdmin(string userId);
	}
}

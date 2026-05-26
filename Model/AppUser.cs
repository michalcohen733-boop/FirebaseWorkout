using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseWorkout.Model
{
	// מחלקת המשתמש - מייצגת משתמש רשום במערכת
	// נשמרת ב-Firebase Realtime Database תחת הנתיב users/
	public class AppUser
	{
		// מזהה ייחודי של המשתמש (מ-Firebase Auth)
		public string Id { get; set; }
		// שם פרטי
		public string? FirstName { get; set; }
		// שם משפחה
		public string? LastName { get; set; }
		// כתובת אימייל - משמשת גם להתחברות
		public string? UserEmail { get; set; }
		// סיסמה
		public string? UserPassword { get; set; }
		// מספר טלפון נייד
		public string? UserMobile { get; set; }
		// תאריך לידה
		public string? UBDate { get; set; }
		// תאריך הרשמה למערכת
		public string? RegDate { get; set; }
		// האם המשתמש הוא מנהל (Admin)
		public bool IsAdmin { get; set; } = false;
		// האם המשתמש הוא איש שירות/אב בית
		public bool IsServicePerson { get; set; } = false;
	}
}

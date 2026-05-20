using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseWorkout.Model
{
	public class AppUser
	{
		public string Id { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? UserEmail { get; set; }
		public string? UserPassword { get; set; }
		public string? UserMobile { get; set; }
		public string? UBDate { get; set; }
		public string? RegDate { get; set; }
		public bool IsAdmin { get; set; } = false;
		public bool IsServicePerson { get; set; } = false;
	}
}

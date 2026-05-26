using CommunityToolkit.Mvvm.ComponentModel;

namespace FirebaseWorkout.Model
{
    // מחלקת סוג תקלה - מייצגת תקלה אפשרית (כמו "מסך", "מקלדת")
    // יורשת מ-ObservableObject כדי שה-CheckBox ב-UI יתעדכן בזמן אמת
    public partial class IssueType : ObservableObject
    {
        // שם סוג התקלה (למשל "Monitor", "Keyboard")
        public string Name { get; set; } = string.Empty;

        // האם התקלה סומנה על ידי המשתמש (CheckBox)
        // ObservableProperty - מעדכן את ה-UI אוטומטית כשהערך משתנה
        [ObservableProperty]
        private bool _isSelected = false;

        // קונסטרקטור ריק - נדרש ל-Firebase deserialization
        public IssueType() { }

        // קונסטרקטור עם שם - ליצירת רשימת תקלות בקוד
        public IssueType(string name)
        {
            Name = name;
        }
    }
}

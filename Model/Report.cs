namespace FirebaseWorkout.Model
{
    // מחלקת דיווח תקלה - מייצגת דיווח על בעיה במחשב
    // נשמרת ב-Firebase תחת הנתיב reports/
    public class Report
    {
        // מזהה ייחודי של הדיווח ב-Firebase
        public string Id { get; set; } = string.Empty;
        // מזהה המחשב שעליו דווחה התקלה
        public string ComputerId { get; set; } = string.Empty;
        // מספר המחשב (להצגה למשתמש)
        public string ComputerNumber { get; set; } = string.Empty;
        // שם החדר/כיתה של המחשב
        public string Room { get; set; } = string.Empty;
        // מזהה המדווח (ID של המשתמש, או ריק אם Guest)
        public string ReporterId { get; set; } = string.Empty;
        // שם המדווח (או "Guest" אם אורח)
        public string ReporterName { get; set; } = string.Empty;
        // רשימת סוגי התקלות שנבחרו (למשל: "מסך", "מקלדת")
        public List<string> IssueTypes { get; set; } = new();
        // תיאור חופשי של תקלה מסוג "אחר"
        public string OtherDescription { get; set; } = string.Empty;
        // תאריך הדיווח
        public string ReportDate { get; set; } = string.Empty;
        // סטטוס הדיווח: Open / InProgress / Closed
        public string Status { get; set; } = "Open";

        // שדה מחושב - מחזיר סיכום של כל התקלות כטקסט
        public string IssuesSummary => IssueTypes?.Count > 0
            ? string.Join(", ", IssueTypes)
            : "No issues listed";
    }
}

using Firebase.Database.Streaming;
using FirebaseWorkout.Model;

namespace FirebaseWorkout.Service.DBService
{
    // ממשק Repository של דיווחים - מגדיר פעולות CRUD על דיווחי תקלות
    public interface IReportRepository
    {
        // יצירת דיווח חדש, מחזיר את ה-ID
        Task<string> CreateAsync(Report report);
        // שליפת דיווח לפי מזהה
        Task<Report?> GetByIdAsync(string id);
        // שליפת כל הדיווחים
        Task<List<Report>> GetAllAsync();
        // שליפת דיווחים פתוחים (Open / InProgress)
        Task<List<Report>> GetOpenReportsAsync();
        // שליפת דיווחים לפי מזהה מחשב
        Task<List<Report>> GetByComputerIdAsync(string computerId);
        // עדכון סטטוס דיווח (Open → InProgress → Closed)
        Task UpdateStatusAsync(string reportId, string newStatus);
        // מחיקת דיווח לפי מזהה
        Task DeleteAsync(string id);
        // הרשמה לעדכונים בזמן אמת על שינויי דיווחים (Rx Observable)
        IObservable<FirebaseEvent<Report>> SubscribeToReportChanges();
    }
}

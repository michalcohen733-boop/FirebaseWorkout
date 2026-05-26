using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Database.Streaming;
using FirebaseWorkout.Model;

namespace FirebaseWorkout.Service.DBService.Firebase
{
    // Repository של דיווחי תקלות - אחראי על כל הפעולות של דיווחים
    // מול Firebase Realtime Database תחת הנתיב reports/
    public class FirebaseReportsRepository : FirebaseRealtimeService, IReportRepository
    {
        // שירות לוגים
        private readonly IAppLogger _appLogger;

        // הקונסטרקטור מקבל שירות לוגים דרך DI
        public FirebaseReportsRepository(IAppLogger appLogger)
        {
            _appLogger = appLogger;
        }

        // יצירת דיווח חדש ב-Firebase - מחזיר את ה-ID
        public async Task<string> CreateAsync(Report report)
        {
            try
            {
                // יצירת מזהה ייחודי אם לא קיים
                if (string.IsNullOrEmpty(report.Id))
                    report.Id = Guid.NewGuid().ToString();

                // הגדרת תאריך דיווח אוטומטי
                if (string.IsNullOrEmpty(report.ReportDate))
                    report.ReportDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                await _firebaseClient!
                    .Child("reports")
                    .Child(report.Id)
                    .PutAsync(report);

                _appLogger.LogDebug($"FirebaseReportsRepository: Report {report.Id} created successfully.");
                return report.Id;
            }
            catch (Exception ex)
            {
                _appLogger.LogDebug($"FirebaseReportsRepository CreateAsync failed: {ex.Message}");
                throw new Exception("Create report failed!");
            }
        }

        // שליפת דיווח לפי מזהה מ-Firebase
        public async Task<Report?> GetByIdAsync(string id)
        {
            try
            {
                var report = await _firebaseClient!
                    .Child("reports")
                    .Child(id)
                    .OnceSingleAsync<Report>();

                _appLogger.LogDebug($"FirebaseReportsRepository: GetByIdAsync {id} completed.");
                return report;
            }
            catch (FirebaseException ex)
            {
                string errorMessage = ex.Message.Contains("401") || ex.Message.Contains("Permission denied")
                    ? "GetByIdAsync failed: Permission denied!"
                    : ex.Message.Contains("404")
                        ? "GetByIdAsync failed: Wrong db path!"
                        : "GetByIdAsync failed: Unknown exception!";

                _appLogger.LogDebug($"FirebaseReportsRepository {errorMessage}");
                throw new Exception(errorMessage);
            }
            catch (Exception ex)
            {
                _appLogger.LogDebug($"FirebaseReportsRepository GetByIdAsync failed: {ex.Message}");
                throw new Exception("Get report failed!");
            }
        }

        // שליפת כל הדיווחים מ-Firebase עם מיפוי שדות מפורש
        public async Task<List<Report>> GetAllAsync()
        {
            try
            {
                var all = await _firebaseClient!
                    .Child("reports")
                    .OnceAsync<Report>();

                _appLogger.LogDebug("FirebaseReportsRepository: GetAllAsync completed.");
                // מיפוי מפורש של כל שדה כולל טיפול ב-null ב-IssueTypes
                return all.Select(r => new Report
                {
                    Id = r.Object.Id,
                    ComputerId = r.Object.ComputerId,
                    ComputerNumber = r.Object.ComputerNumber,
                    Room = r.Object.Room,
                    ReporterId = r.Object.ReporterId,
                    ReporterName = r.Object.ReporterName,
                    IssueTypes = r.Object.IssueTypes ?? new List<string>(),
                    OtherDescription = r.Object.OtherDescription,
                    ReportDate = r.Object.ReportDate,
                    Status = r.Object.Status
                }).ToList();
            }
            catch (Exception ex)
            {
                _appLogger.LogDebug($"FirebaseReportsRepository GetAllAsync failed: {ex.Message}");
                throw new Exception("Get all reports failed!");
            }
        }

        // שליפת דיווחים פתוחים (Open או InProgress) ממוינים לפי תאריך
        public async Task<List<Report>> GetOpenReportsAsync()
        {
            try
            {
                var all = await _firebaseClient!
                    .Child("reports")
                    .OnceAsync<Report>();

                // סינון רק דיווחים פתוחים ומיון מהחדש לישן
                var openReports = all
                    .Select(r => r.Object)
                    .Where(r => r.Status == "Open" || r.Status == "InProgress")
                    .OrderByDescending(r => r.ReportDate)
                    .ToList();

                _appLogger.LogDebug($"FirebaseReportsRepository: GetOpenReportsAsync returned {openReports.Count} reports.");
                return openReports;
            }
            catch (Exception ex)
            {
                _appLogger.LogDebug($"FirebaseReportsRepository GetOpenReportsAsync failed: {ex.Message}");
                throw new Exception("Get open reports failed!");
            }
        }

        // שליפת כל הדיווחים של מחשב ספציפי לפי מזהה המחשב
        public async Task<List<Report>> GetByComputerIdAsync(string computerId)
        {
            try
            {
                var all = await _firebaseClient!
                    .Child("reports")
                    .OnceAsync<Report>();

                var computerReports = all
                    .Select(r => r.Object)
                    .Where(r => r.ComputerId == computerId)
                    .OrderByDescending(r => r.ReportDate)
                    .ToList();

                _appLogger.LogDebug($"FirebaseReportsRepository: GetByComputerIdAsync for {computerId} returned {computerReports.Count} reports.");
                return computerReports;
            }
            catch (Exception ex)
            {
                _appLogger.LogDebug($"FirebaseReportsRepository GetByComputerIdAsync failed: {ex.Message}");
                throw new Exception("Get reports by computer failed!");
            }
        }

        // עדכון סטטוס דיווח (למשל Open → Closed) - שולח רק את שדה Status
        public async Task UpdateStatusAsync(string reportId, string newStatus)
        {
            try
            {
                await _firebaseClient!
                    .Child("reports")
                    .Child(reportId)
                    .PatchAsync(new { Status = newStatus });

                _appLogger.LogDebug($"FirebaseReportsRepository: Report {reportId} status updated to '{newStatus}'.");
            }
            catch (Exception ex)
            {
                _appLogger.LogDebug($"FirebaseReportsRepository UpdateStatusAsync failed: {ex.Message}");
                throw new Exception("Update report status failed!");
            }
        }

        // מחיקת דיווח לפי מזהה מ-Firebase
        public async Task DeleteAsync(string id)
        {
            try
            {
                await _firebaseClient!
                    .Child("reports")
                    .Child(id)
                    .DeleteAsync();

                _appLogger.LogDebug($"FirebaseReportsRepository: Report {id} deleted successfully.");
            }
            catch (Exception ex)
            {
                _appLogger.LogDebug($"FirebaseReportsRepository DeleteAsync failed: {ex.Message}");
                throw new Exception("Delete report failed!");
            }
        }

        // הרשמה לעדכונים בזמן אמת - מחזיר Observable שמקבל שינויים מ-Firebase
        public IObservable<FirebaseEvent<Report>> SubscribeToReportChanges()
        {
            try
            {
                return _firebaseClient!
                    .Child("reports")
                    .AsObservable<Report>();
            }
            catch (Exception ex)
            {
                _appLogger.LogError("SubscribeToReportChanges failed: " + ex.Message);
                throw new Exception("SubscribeToReportChanges failed!");
            }
        }
    }
}

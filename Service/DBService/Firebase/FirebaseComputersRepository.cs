using Firebase.Database;
using Firebase.Database.Query;
using FirebaseWorkout.Model;

namespace FirebaseWorkout.Service.DBService.Firebase
{
    // Repository של מחשבים - אחראי על כל הפעולות של מחשבים
    // מול Firebase Realtime Database תחת הנתיב computers/
    public class FirebaseComputersRepository : FirebaseRealtimeService, IComputerRepository
    {
        // שירות לוגים
        private readonly IAppLogger _appLogger;

        // הקונסטרקטור מקבל שירות לוגים דרך DI
        public FirebaseComputersRepository(IAppLogger appLogger)
        {
            _appLogger = appLogger;
        }

        // יצירת מחשב חדש ב-Firebase - מחזיר את ה-ID
        public async Task<string> CreateAsync(Computer computer)
        {
            try
            {
                // יצירת מזהה ייחודי אם לא קיים
                if (string.IsNullOrEmpty(computer.Id))
                    computer.Id = Guid.NewGuid().ToString();

                // הגדרת קוד QR - ברירת מחדל זהה ל-ID
                if (string.IsNullOrEmpty(computer.QRCode))
                    computer.QRCode = computer.Id;

                await _firebaseClient!
                    .Child("computers")
                    .Child(computer.Id)
                    .PutAsync(computer);

                _appLogger.LogDebug($"FirebaseComputersRepository: Computer {computer.ComputerNumber} created successfully.");
                return computer.Id;
            }
            catch (Exception ex)
            {
                _appLogger.LogDebug($"FirebaseComputersRepository CreateAsync failed: {ex.Message}");
                throw new Exception("Create computer failed!");
            }
        }

        // שליפת מחשב לפי מזהה מ-Firebase
        public async Task<Computer?> GetByIdAsync(string id)
        {
            try
            {
                var computer = await _firebaseClient!
                    .Child("computers")
                    .Child(id)
                    .OnceSingleAsync<Computer>();

                _appLogger.LogDebug($"FirebaseComputersRepository: GetByIdAsync {id} completed.");
                return computer;
            }
            catch (FirebaseException ex)
            {
                string errorMessage = ex.Message.Contains("401") || ex.Message.Contains("Permission denied")
                    ? "GetByIdAsync failed: Permission denied!"
                    : ex.Message.Contains("404")
                        ? "GetByIdAsync failed: Wrong db path!"
                        : "GetByIdAsync failed: Unknown exception!";

                _appLogger.LogDebug($"FirebaseComputersRepository {errorMessage}");
                throw new Exception(errorMessage);
            }
            catch (Exception ex)
            {
                _appLogger.LogDebug($"FirebaseComputersRepository GetByIdAsync failed: {ex.Message}");
                throw new Exception("Get computer failed!");
            }
        }

        // חיפוש מחשב לפי קוד QR - שולף את כל המחשבים ומחפש התאמה
        // מחפש גם לפי QRCode וגם לפי ID (גיבוי)
        public async Task<Computer?> GetByQRCodeAsync(string qrCode)
        {
            try
            {
                // שליפת כל המחשבים מ-Firebase
                var all = await _firebaseClient!
                    .Child("computers")
                    .OnceAsync<Computer>();

                // חיפוש התאמה לפי QRCode או ID
                var match = all
                    .Select(c => c.Object)
                    .FirstOrDefault(c => c.QRCode == qrCode || c.Id == qrCode);

                _appLogger.LogDebug($"FirebaseComputersRepository: GetByQRCodeAsync '{qrCode}' — {(match != null ? "found" : "not found")}.");
                return match;
            }
            catch (Exception ex)
            {
                _appLogger.LogDebug($"FirebaseComputersRepository GetByQRCodeAsync failed: {ex.Message}");
                throw new Exception("Get computer by QR code failed!");
            }
        }

        // שליפת כל המחשבים מ-Firebase עם מיפוי שדות מפורש
        public async Task<List<Computer>> GetAllAsync()
        {
            try
            {
                var all = await _firebaseClient!
                    .Child("computers")
                    .OnceAsync<Computer>();

                _appLogger.LogDebug("FirebaseComputersRepository: GetAllAsync completed.");
                // מיפוי מפורש של כל שדה (ולא .Select(x => x.Object))
                return all.Select(c => new Computer
                {
                    Id = c.Object.Id,
                    ComputerNumber = c.Object.ComputerNumber,
                    Room = c.Object.Room,
                    QRCode = c.Object.QRCode,
                    IsActive = c.Object.IsActive
                }).ToList();
            }
            catch (Exception ex)
            {
                _appLogger.LogDebug($"FirebaseComputersRepository GetAllAsync failed: {ex.Message}");
                throw new Exception("Get all computers failed!");
            }
        }

        // עדכון פרטי מחשב - שולח רק את השדות שהשתנו (Patch)
        public async Task UpdateAsync(Computer computer)
        {
            try
            {
                await _firebaseClient!
                    .Child("computers")
                    .Child(computer.Id)
                    .PatchAsync(new
                    {
                        computer.ComputerNumber,
                        computer.Room,
                        computer.QRCode,
                        computer.IsActive
                    });

                _appLogger.LogDebug($"FirebaseComputersRepository: Computer {computer.Id} updated successfully.");
            }
            catch (Exception ex)
            {
                _appLogger.LogDebug($"FirebaseComputersRepository UpdateAsync failed: {ex.Message}");
                throw new Exception("Update computer failed!");
            }
        }

        // מחיקת מחשב לפי מזהה מ-Firebase
        public async Task DeleteAsync(string id)
        {
            try
            {
                await _firebaseClient!
                    .Child("computers")
                    .Child(id)
                    .DeleteAsync();

                _appLogger.LogDebug($"FirebaseComputersRepository: Computer {id} deleted successfully.");
            }
            catch (Exception ex)
            {
                _appLogger.LogDebug($"FirebaseComputersRepository DeleteAsync failed: {ex.Message}");
                throw new Exception("Delete computer failed!");
            }
        }
    }
}

using FirebaseWorkout.Model;

namespace FirebaseWorkout.Service.DBService
{
    // ממשק Repository של מחשבים - מגדיר פעולות CRUD על מחשבים
    public interface IComputerRepository
    {
        // יצירת מחשב חדש, מחזיר את ה-ID
        Task<string> CreateAsync(Computer computer);
        // שליפת מחשב לפי מזהה
        Task<Computer?> GetByIdAsync(string id);
        // שליפת מחשב לפי קוד QR (או לפי ID)
        Task<Computer?> GetByQRCodeAsync(string qrCode);
        // שליפת כל המחשבים
        Task<List<Computer>> GetAllAsync();
        // עדכון פרטי מחשב
        Task UpdateAsync(Computer computer);
        // מחיקת מחשב לפי מזהה
        Task DeleteAsync(string id);
    }
}

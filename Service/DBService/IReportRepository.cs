using Firebase.Database.Streaming;
using FirebaseWorkout.Model;

namespace FirebaseWorkout.Service.DBService
{
    public interface IReportRepository
    {
        Task<string> CreateAsync(Report report);
        Task<Report?> GetByIdAsync(string id);
        Task<List<Report>> GetAllAsync();
        Task<List<Report>> GetOpenReportsAsync();
        Task<List<Report>> GetByComputerIdAsync(string computerId);
        Task UpdateStatusAsync(string reportId, string newStatus);
        Task DeleteAsync(string id);
        IObservable<FirebaseEvent<Report>> SubscribeToReportChanges();
    }
}

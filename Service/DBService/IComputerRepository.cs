using FirebaseWorkout.Model;

namespace FirebaseWorkout.Service.DBService
{
    public interface IComputerRepository
    {
        Task<string> CreateAsync(Computer computer);
        Task<Computer?> GetByIdAsync(string id);
        Task<Computer?> GetByQRCodeAsync(string qrCode);
        Task<List<Computer>> GetAllAsync();
        Task UpdateAsync(Computer computer);
        Task DeleteAsync(string id);
    }
}

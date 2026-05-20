using Firebase.Database;
using Firebase.Database.Query;
using FirebaseWorkout.Model;

namespace FirebaseWorkout.Service.DBService.Firebase
{
    public class FirebaseComputersRepository : FirebaseRealtimeService, IComputerRepository
    {
        private readonly IAppLogger _appLogger;

        public FirebaseComputersRepository(IAppLogger appLogger)
        {
            _appLogger = appLogger;
        }

        public async Task<string> CreateAsync(Computer computer)
        {
            try
            {
                if (string.IsNullOrEmpty(computer.Id))
                    computer.Id = Guid.NewGuid().ToString();

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

        public async Task<Computer?> GetByQRCodeAsync(string qrCode)
        {
            try
            {
                var all = await _firebaseClient!
                    .Child("computers")
                    .OnceAsync<Computer>();

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

        public async Task<List<Computer>> GetAllAsync()
        {
            try
            {
                var all = await _firebaseClient!
                    .Child("computers")
                    .OnceAsync<Computer>();

                _appLogger.LogDebug("FirebaseComputersRepository: GetAllAsync completed.");
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

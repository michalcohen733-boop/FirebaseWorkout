using CommunityToolkit.Mvvm.ComponentModel;
using FirebaseWorkout.Model;
using FirebaseWorkout.Service;
using FirebaseWorkout.Service.DBService;
using System.Collections.ObjectModel;

namespace FirebaseWorkout.ViewModels
{
    // ViewModel של מסך שירות - מציג רשימת דיווחים פתוחים בזמן אמת
    // נגיש ל-Admin ול-ServicePerson (אב בית)
    public partial class ServicePageViewModel : ObservableObject
    {
        // Repository דיווחים
        private readonly IReportRepository _reportRepo;
        // שירות התראות
        private readonly IAlertService _alertService;
        // שירות לוגים
        private readonly IAppLogger _appLogger;

        // מנוי לעדכונים בזמן אמת מ-Firebase (Rx Observable)
        private IDisposable? _subscription;

        // רשימת הדיווחים הפתוחים (מוצגת ב-CollectionView)
        [ObservableProperty]
        private ObservableCollection<Report> _reports;

        // טקסט מונה דיווחים: "3 reports"
        [ObservableProperty]
        private string _reportsCount = "0 reports";

        // הקונסטרקטור מקבל שירותים דרך DI
        public ServicePageViewModel(
            IReportRepository reportRepo,
            IAlertService alertService,
            IAppLogger appLogger)
        {
            _reportRepo = reportRepo;
            _alertService = alertService;
            _appLogger = appLogger;
            Reports = new ObservableCollection<Report>();
        }

        // טוען דיווחים פתוחים מ-Firebase ומעדכן את הרשימה
        public async Task LoadReportsAsync()
        {
            try
            {
                var openReports = await _reportRepo.GetOpenReportsAsync();
                Reports.Clear();
                foreach (var r in openReports)
                    Reports.Add(r);
                UpdateCount();
            }
            catch (Exception ex)
            {
                _appLogger.LogDebug($"LoadReportsAsync failed: {ex.Message}");
                await _alertService.ShowAlertAsync("Error", "Failed to load reports", "OK");
            }
        }

        // הרשמה לעדכונים בזמן אמת - כל שינוי ב-Firebase טוען מחדש את הרשימה
        public void SubscribeToChanges()
        {
            try
            {
                _subscription = _reportRepo.SubscribeToReportChanges()
                    .Subscribe(
                        _ => MainThread.BeginInvokeOnMainThread(async () =>
                            await LoadReportsAsync()),
                        ex => _appLogger.LogDebug($"SubscribeToChanges error: {ex.Message}"));
            }
            catch (Exception ex)
            {
                _appLogger.LogDebug($"SubscribeToChanges failed: {ex.Message}");
            }
        }

        // ביטול ההרשמה לעדכונים (כשיוצאים מהמסך)
        public void UnsubscribeFromChanges()
        {
            _subscription?.Dispose();
            _subscription = null;
        }

        // טיפול בלחיצה על דיווח - מציע לסגור אותו
        // מציג Alert עם אישור ומעדכן את הסטטוס ל-Closed
        public async Task HandleReportSelectedAsync(Report report)
        {
            bool answer = await Shell.Current.DisplayAlert(
                "Mark as Closed",
                $"Mark report for {report.ComputerNumber} as resolved?",
                "Yes",
                "Cancel");

            if (answer)
            {
                try
                {
                    await _reportRepo.UpdateStatusAsync(report.Id, "Closed");
                    Reports.Remove(report);
                    UpdateCount();
                }
                catch (Exception ex)
                {
                    _appLogger.LogDebug($"Update status failed: {ex.Message}");
                    await _alertService.ShowAlertAsync("Error", "Failed to update report", "OK");
                }
            }
        }

        // עדכון טקסט מונה הדיווחים
        private void UpdateCount()
        {
            ReportsCount = Reports.Count == 1
                ? "1 report"
                : $"{Reports.Count} reports";
        }
    }
}

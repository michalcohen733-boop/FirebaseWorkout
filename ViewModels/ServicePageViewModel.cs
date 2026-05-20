using CommunityToolkit.Mvvm.ComponentModel;
using FirebaseWorkout.Model;
using FirebaseWorkout.Service;
using FirebaseWorkout.Service.DBService;
using System.Collections.ObjectModel;

namespace FirebaseWorkout.ViewModels
{
    public partial class ServicePageViewModel : ObservableObject
    {
        private readonly IReportRepository _reportRepo;
        private readonly IAlertService _alertService;
        private readonly IAppLogger _appLogger;

        private IDisposable? _subscription;

        [ObservableProperty]
        private ObservableCollection<Report> _reports;

        [ObservableProperty]
        private string _reportsCount = "0 reports";

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

        public void UnsubscribeFromChanges()
        {
            _subscription?.Dispose();
            _subscription = null;
        }

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

        private void UpdateCount()
        {
            ReportsCount = Reports.Count == 1
                ? "1 report"
                : $"{Reports.Count} reports";
        }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FirebaseWorkout.Model;
using FirebaseWorkout.Service;
using FirebaseWorkout.Service.DBService;
using Microsoft.Maui.Graphics;
using System.Collections.ObjectModel;

namespace FirebaseWorkout.ViewModels
{
    public partial class ReportPageViewModel : ObservableObject, IQueryAttributable
    {
        private readonly IReportRepository _reportRepo;
        private readonly IAlertService _alertService;
        private readonly IAppLogger _appLogger;

        [ObservableProperty]
        private Computer _computer;

        [ObservableProperty]
        private string _computerDisplayInfo;

        [ObservableProperty]
        private ObservableCollection<IssueType> _issueOptions;

        [ObservableProperty]
        private string _otherDescription;

        [ObservableProperty]
        private bool _isOtherSelected;

        [ObservableProperty]
        private string _statusMessage;

        [ObservableProperty]
        private bool _hasStatusMessage;

        [ObservableProperty]
        private Color _statusColor = Color.FromArgb("#EF4444");

        public ReportPageViewModel(
            IReportRepository reportRepo,
            IAlertService alertService,
            IAppLogger appLogger)
        {
            _reportRepo = reportRepo;
            _alertService = alertService;
            _appLogger = appLogger;

            IssueOptions = new ObservableCollection<IssueType>
            {
                new IssueType("the mouse"),
                new IssueType("the Keyboard"),
                new IssueType("the Internet"),
                new IssueType("the Screen not turning on"),
                new IssueType("the Projector shows 'No Signal'"),
                new IssueType("Cannot access school websites"),
                new IssueType("No sound from speakers"),
                new IssueType("other")
            };

            foreach (var issue in IssueOptions)
            {
                issue.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(IssueType.IsSelected))
                    {
                        var otherIssue = IssueOptions.FirstOrDefault(i => i.Name == "other");
                        IsOtherSelected = otherIssue?.IsSelected ?? false;
                    }
                };
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("computer", out var obj) && obj is Computer computer)
            {
                Computer = computer;
                ComputerDisplayInfo = $"{computer.ComputerNumber} / {computer.Room}";
            }
        }

        [RelayCommand]
        private async Task SubmitReport()
        {
            var selected = IssueOptions
                .Where(i => i.IsSelected)
                .Select(i => i.Name)
                .ToList();

            if (selected.Count == 0)
            {
                StatusMessage = "Please select at least one issue.";
                StatusColor = Color.FromArgb("#EF4444");
                HasStatusMessage = true;
                return;
            }

            if (selected.Contains("other") && string.IsNullOrWhiteSpace(OtherDescription))
            {
                StatusMessage = "Please describe the 'other' issue.";
                StatusColor = Color.FromArgb("#EF4444");
                HasStatusMessage = true;
                return;
            }

            try
            {
                var currentUser = (App.Current as App)?.CurrentUser;

                var report = new Report
                {
                    ComputerId = Computer.Id,
                    ComputerNumber = Computer.ComputerNumber,
                    Room = Computer.Room,
                    ReporterId = currentUser?.Id ?? string.Empty,
                    ReporterName = currentUser != null
                        ? $"{currentUser.FirstName} {currentUser.LastName}"
                        : "Guest",
                    IssueTypes = selected,
                    OtherDescription = selected.Contains("other") ? OtherDescription : string.Empty,
                    Status = "Open"
                };

                await _reportRepo.CreateAsync(report);

                await _alertService.ShowAlertAsync("Success", "Your report has been submitted!", "OK");

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                _appLogger.LogDebug($"SubmitReport failed: {ex.Message}");
                StatusMessage = "Failed to submit. Please try again.";
                StatusColor = Color.FromArgb("#EF4444");
                HasStatusMessage = true;
            }
        }
    }
}

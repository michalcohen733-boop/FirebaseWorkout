using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FirebaseWorkout.Model;
using FirebaseWorkout.Service;
using FirebaseWorkout.Service.DBService;
using Microsoft.Maui.Graphics;
using System.Collections.ObjectModel;

namespace FirebaseWorkout.ViewModels
{
    // ViewModel של מסך דיווח תקלה - מציג 8 סוגי תקלות כ-CheckBox
    // מממש IQueryAttributable כדי לקבל את אובייקט המחשב מהמסך הקודם
    public partial class ReportPageViewModel : ObservableObject, IQueryAttributable
    {
        // Repository דיווחים לשמירה ב-Firebase
        private readonly IReportRepository _reportRepo;
        // שירות התראות להצגת הודעות
        private readonly IAlertService _alertService;
        // שירות לוגים
        private readonly IAppLogger _appLogger;

        // אובייקט המחשב שעליו מדווחים (מתקבל מהמסך הקודם)
        [ObservableProperty]
        private Computer _computer;

        // טקסט תצוגה: "מספר מחשב / חדר"
        [ObservableProperty]
        private string _computerDisplayInfo;

        // רשימת סוגי תקלות אפשריות (8 אפשרויות + CheckBox)
        [ObservableProperty]
        private ObservableCollection<IssueType> _issueOptions;

        // תיאור חופשי לתקלה מסוג "אחר"
        [ObservableProperty]
        private string _otherDescription;

        // האם "other" מסומן - מציג שדה טקסט חופשי
        [ObservableProperty]
        private bool _isOtherSelected;

        // הודעת סטטוס/שגיאה
        [ObservableProperty]
        private string _statusMessage;

        // האם יש הודעה להציג
        [ObservableProperty]
        private bool _hasStatusMessage;

        // צבע הודעת הסטטוס (אדום=שגיאה, ירוק=הצלחה)
        [ObservableProperty]
        private Color _statusColor = Color.FromArgb("#EF4444");

        // הקונסטרקטור מאתחל את רשימת סוגי התקלות ומאזין לשינויים ב-CheckBox
        public ReportPageViewModel(
            IReportRepository reportRepo,
            IAlertService alertService,
            IAppLogger appLogger)
        {
            _reportRepo = reportRepo;
            _alertService = alertService;
            _appLogger = appLogger;

            // 8 סוגי תקלות אפשריות
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

            // האזנה לשינויים - כשבוחרים "other" מציגים שדה תיאור חופשי
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

        // מקבל את אובייקט המחשב שהועבר מהמסך הקודם (סריקה/הזנה ידנית)
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("computer", out var obj) && obj is Computer computer)
            {
                Computer = computer;
                ComputerDisplayInfo = $"{computer.ComputerNumber} / {computer.Room}";
            }
        }

        // שליחת דיווח תקלה - בודק קלט, בונה אובייקט Report ושומר ב-Firebase
        [RelayCommand]
        private async Task SubmitReport()
        {
            // איסוף התקלות שנבחרו
            var selected = IssueOptions
                .Where(i => i.IsSelected)
                .Select(i => i.Name)
                .ToList();

            // בדיקה שנבחרה לפחות תקלה אחת
            if (selected.Count == 0)
            {
                StatusMessage = "Please select at least one issue.";
                StatusColor = Color.FromArgb("#EF4444");
                HasStatusMessage = true;
                return;
            }

            // בדיקה שאם נבחר "other" יש תיאור
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

                // בניית אובייקט דיווח חדש
                var report = new Report
                {
                    ComputerId = Computer.Id,
                    ComputerNumber = Computer.ComputerNumber,
                    Room = Computer.Room,
                    ReporterId = currentUser?.Id ?? string.Empty,
                    // אם Guest - הוא "Guest", אחרת שם מלא
                    ReporterName = currentUser != null
                        ? $"{currentUser.FirstName} {currentUser.LastName}"
                        : "Guest",
                    IssueTypes = selected,
                    OtherDescription = selected.Contains("other") ? OtherDescription : string.Empty,
                    Status = "Open"
                };

                // שמירה ב-Firebase
                await _reportRepo.CreateAsync(report);

                await _alertService.ShowAlertAsync("Success", "Your report has been submitted!", "OK");

                // חזרה למסך הסריקה
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

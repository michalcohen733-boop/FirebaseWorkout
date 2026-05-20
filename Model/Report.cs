namespace FirebaseWorkout.Model
{
    public class Report
    {
        public string Id { get; set; } = string.Empty;
        public string ComputerId { get; set; } = string.Empty;
        public string ComputerNumber { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public string ReporterId { get; set; } = string.Empty;
        public string ReporterName { get; set; } = string.Empty;
        public List<string> IssueTypes { get; set; } = new();
        public string OtherDescription { get; set; } = string.Empty;
        public string ReportDate { get; set; } = string.Empty;
        public string Status { get; set; } = "Open";

        public string IssuesSummary => IssueTypes?.Count > 0
            ? string.Join(", ", IssueTypes)
            : "No issues listed";
    }
}

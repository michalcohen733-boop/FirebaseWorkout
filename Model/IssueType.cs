using CommunityToolkit.Mvvm.ComponentModel;

namespace FirebaseWorkout.Model
{
    public partial class IssueType : ObservableObject
    {
        public string Name { get; set; } = string.Empty;

        [ObservableProperty]
        private bool _isSelected = false;

        public IssueType() { }

        public IssueType(string name)
        {
            Name = name;
        }
    }
}

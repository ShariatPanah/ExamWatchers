namespace ExamWatchers.ViewModels
{
    public class ToggleWatcherPresenceResponseViewModel
    {
        public string ActionText { get; set; }
        public bool ShowAction { get; set; }
        public string RemainingCapacityText { get; set; }
        public bool IsOutOfCapacity { get; set; }
    }
}
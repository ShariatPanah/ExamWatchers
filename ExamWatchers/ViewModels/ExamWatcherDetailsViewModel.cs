using System;

namespace ExamWatchers.ViewModels
{
    public class ExamWatcherDetailsViewModel
    {
        public string WatcherCode { get; internal set; }
        public string WatcherName { get; internal set; }
        public DateTime SubmitDate { get; internal set; }
    }
}
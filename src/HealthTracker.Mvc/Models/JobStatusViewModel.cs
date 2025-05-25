using HealthTracker.Entities.Logging;

namespace HealthTracker.Mvc.Models
{
    public class JobStatusViewModel : DateBasedReportViewModelBase<JobStatus>
    {
        public IEnumerable<JobStatus> JobStatuses => Entities;
    }
}

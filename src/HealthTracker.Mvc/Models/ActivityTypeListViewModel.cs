using HealthTracker.Entities.Measurements;

namespace HealthTracker.Mvc.Models
{
    public class ActivityTypeListViewModel : ListViewModelBase<ActivityType>
    {
        public IEnumerable<ActivityType> ActivityTypes => Entities;
    }
}
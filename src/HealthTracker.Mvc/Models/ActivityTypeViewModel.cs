using HealthTracker.Entities.Measurements;

namespace HealthTracker.Mvc.Models
{
    public class ActivityTypeViewModel
    {
        public ActivityType ActivityType { get; set; } = new();
        public string Action { get; set; }
    }
}
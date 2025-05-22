using HealthTracker.Entities.Measurements;

namespace HealthTracker.Mvc.Models
{
    public class BloodGlucoseListViewModel : FilteredViewModelBase<BloodGlucoseMeasurement>
    {
        public IEnumerable<BloodGlucoseMeasurement> Measurements => Entities;
    }
}
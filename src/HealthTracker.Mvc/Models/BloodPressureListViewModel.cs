using HealthTracker.Entities.Measurements;

namespace HealthTracker.Mvc.Models
{
    public class BloodPressureListViewModel : FilteredViewModelBase<BloodPressureMeasurement>
    {
        public IEnumerable<BloodPressureMeasurement> Measurements => Entities;
    }
}
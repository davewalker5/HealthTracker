using HealthTracker.Entities.Measurements;

namespace HealthTracker.Mvc.Models
{
    public class BloodPressureListViewModel : ListViewModelBase<BloodPressureMeasurement>
    {
        public IEnumerable<BloodPressureMeasurement> Measurements => Entities;
    }
}
using HealthTracker.Entities.Measurements;

namespace HealthTracker.Mvc.Models
{
    public class BloodPressureListViewModel : MeasurementListViewModelBase<BloodPressureMeasurement>
    {
        public IEnumerable<BloodPressureMeasurement> Measurements => Entities;
    }
}
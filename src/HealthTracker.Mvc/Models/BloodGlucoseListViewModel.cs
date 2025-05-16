using HealthTracker.Entities.Measurements;

namespace HealthTracker.Mvc.Models
{
    public class BloodGlucoseListViewModel : MeasurementListViewModelBase<BloodGlucoseMeasurement>
    {
        public IEnumerable<BloodGlucoseMeasurement> Measurements => Entities;
    }
}
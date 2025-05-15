using HealthTracker.Entities.Measurements;

namespace HealthTracker.Mvc.Models
{
    public class BloodOxygenSaturationListViewModel : MeasurementListViewModelBase<BloodOxygenSaturationMeasurement>
    {
        public IEnumerable<BloodOxygenSaturationMeasurement> Measurements => Entities;
    }
}
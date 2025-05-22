using HealthTracker.Entities.Measurements;

namespace HealthTracker.Mvc.Models
{
    public class BloodOxygenSaturationListViewModel : FilteredViewModelBase<BloodOxygenSaturationMeasurement>
    {
        public IEnumerable<BloodOxygenSaturationMeasurement> Measurements => Entities;
    }
}
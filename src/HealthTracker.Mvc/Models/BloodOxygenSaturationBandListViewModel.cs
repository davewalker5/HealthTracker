using HealthTracker.Entities.Measurements;

namespace HealthTracker.Mvc.Models
{
    public class BloodOxygenSaturationBandListViewModel : ReferenceDataListViewModel
    {
        public IEnumerable<BloodOxygenSaturationBand> Bands { get; set; }
    }
}
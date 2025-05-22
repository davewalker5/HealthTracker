using HealthTracker.Entities.Measurements;

namespace HealthTracker.Mvc.Models
{
    public class BloodPressureBandListViewModel : ReferenceDataListViewModel
    {
        public IEnumerable<BloodPressureBand> Bands { get; set; }
    }
}
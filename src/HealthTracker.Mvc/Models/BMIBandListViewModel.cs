using HealthTracker.Entities.Measurements;

namespace HealthTracker.Mvc.Models
{
    public class BMIBandListViewModel : ReferenceDataListViewModel
    {
        public IEnumerable<BMIBand> Bands { get; set; }
    }
}
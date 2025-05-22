using HealthTracker.Entities.Measurements;

namespace HealthTracker.Mvc.Models
{
    public class WeightListViewModel : FilteredViewModelBase<WeightMeasurement>
    {
        public IEnumerable<WeightMeasurement> Measurements => Entities;
    }
}
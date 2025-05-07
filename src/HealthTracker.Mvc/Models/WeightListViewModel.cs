using HealthTracker.Entities.Measurements;

namespace HealthTracker.Mvc.Models
{
    public class WeightListViewModel : ListViewModelBase<WeightMeasurement>
    {
        public IEnumerable<WeightMeasurement> Measurements => Entities;
    }
}
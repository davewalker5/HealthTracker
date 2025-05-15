using HealthTracker.Entities.Measurements;

namespace HealthTracker.Mvc.Models
{
    public class WeightListViewModel : MeasurementListViewModelBase<WeightMeasurement>
    {
        public IEnumerable<WeightMeasurement> Measurements => Entities;
    }
}
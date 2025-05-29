using HealthTracker.Entities.Measurements;

namespace HealthTracker.Mvc.Models
{
    public class AlcoholConsumptionListViewModel : FilteredViewModelBase<AlcoholConsumptionMeasurement>
    {
        public IEnumerable<AlcoholConsumptionMeasurement> Measurements => Entities;
    }
}
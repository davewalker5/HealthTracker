using HealthTracker.Entities.Measurements;

namespace HealthTracker.Mvc.Models
{
    public class BeverageConsumptionListViewModel : FilteredViewModelBase<BeverageConsumptionMeasurement>
    {
        public IEnumerable<BeverageConsumptionMeasurement> Measurements => Entities;
    }
}
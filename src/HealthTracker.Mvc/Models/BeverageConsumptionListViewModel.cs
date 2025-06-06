using HealthTracker.Entities.Food;

namespace HealthTracker.Mvc.Models
{
    public class BeverageConsumptionListViewModel : FilteredViewModelBase<BeverageConsumptionMeasurement>
    {
        public IEnumerable<BeverageConsumptionMeasurement> Measurements => Entities;
    }
}
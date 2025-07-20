using HealthTracker.Entities.Food;

namespace HealthTracker.Mvc.Models
{
    public class MealConsumptionListViewModel : FilteredViewModelBase<MealConsumptionMeasurement>
    {
        public IEnumerable<MealConsumptionMeasurement> Measurements => Entities;
    }
}
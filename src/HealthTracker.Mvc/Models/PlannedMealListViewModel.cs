using HealthTracker.Entities.Food;

namespace HealthTracker.Mvc.Models
{
    public class PlannedMealListViewModel : FilteredViewModelBase<PlannedMeal>
    {
        public IEnumerable<PlannedMeal> Schedule => Entities;
    }
}
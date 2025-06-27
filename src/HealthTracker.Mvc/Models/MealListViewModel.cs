using HealthTracker.Entities.Food;

namespace HealthTracker.Mvc.Models
{
    public class MealListViewModel : FilteredByFoodSourceViewModelBase<Meal>
    {
        public IEnumerable<Meal> Meals => Entities;
    }
}
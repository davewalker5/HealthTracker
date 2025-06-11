using HealthTracker.Entities.Food;

namespace HealthTracker.Mvc.Models
{
    public class FoodItemListViewModel : FilteredByFoodCategoryViewModelBase<FoodItem>
    {
        public IEnumerable<FoodItem> Items => Entities;
    }
}
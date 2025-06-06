using HealthTracker.Entities.Food;

namespace HealthTracker.Mvc.Models
{
    public class FoodCategoryListViewModel : ListViewModelBase<FoodCategory>
    {
        public IEnumerable<FoodCategory> FoodCategories => Entities;
    }
}
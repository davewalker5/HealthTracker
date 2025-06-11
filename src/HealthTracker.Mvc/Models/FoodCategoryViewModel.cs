using HealthTracker.Entities.Food;

namespace HealthTracker.Mvc.Models
{
    public class FoodCategoryViewModel
    {
        public FoodCategory FoodCategory { get; set; } = new();
        public string Action { get; set; }
    }
}
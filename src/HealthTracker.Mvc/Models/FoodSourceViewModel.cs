using HealthTracker.Entities.Food;

namespace HealthTracker.Mvc.Models
{
    public class FoodSourceViewModel
    {
        public FoodSource FoodSource { get; set; } = new();
        public string Action { get; set; }
    }
}
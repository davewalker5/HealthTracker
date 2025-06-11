using HealthTracker.Entities.Food;

namespace HealthTracker.Mvc.Models
{
    public class BeverageViewModel
    {
        public Beverage Beverage { get; set; } = new();
        public string Action { get; set; }
    }
}
using HealthTracker.Entities.Food;

namespace HealthTracker.Mvc.Models
{
    public class NutritionalValueTableViewModel
    {
        public decimal? Portion { get; set; } = null;
        public NutritionalValue Values { get; set; }
    }
}
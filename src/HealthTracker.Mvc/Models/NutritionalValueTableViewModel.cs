using HealthTracker.Entities;
using HealthTracker.Entities.Food;

namespace HealthTracker.Mvc.Models
{
    public class NutritionalValueTableViewModel : HealthTrackerEntityBase
    {
        public decimal? Portion { get; set; } = null;
        public NutritionalValue Values { get; set; }
    }
}
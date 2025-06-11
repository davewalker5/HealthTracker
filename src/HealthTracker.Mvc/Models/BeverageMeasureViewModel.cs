using HealthTracker.Entities.Food;

namespace HealthTracker.Mvc.Models
{
    public class BeverageMeasureViewModel
    {
        public BeverageMeasure Measure { get; set; } = new();
        public string Action { get; set; }
    }
}
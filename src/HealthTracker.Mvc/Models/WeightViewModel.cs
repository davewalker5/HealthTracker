using HealthTracker.Entities.Measurements;
using HealthTracker.Mvc.Interfaces;

namespace HealthTracker.Mvc.Models
{
    public class WeightViewModel : IMeasurementPersonViewModel
    {
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public WeightMeasurement Measurement { get; set; } = new();
    }
}
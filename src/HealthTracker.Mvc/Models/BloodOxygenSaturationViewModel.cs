using HealthTracker.Entities.Measurements;
using HealthTracker.Mvc.Interfaces;

namespace HealthTracker.Mvc.Models
{
    public class BloodOxygenSaturationViewModel : IMeasurementPersonViewModel
    {
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public BloodOxygenSaturationMeasurement Measurement { get; set; } = new();
    }
}
using HealthTracker.Entities.Measurements;
using HealthTracker.Mvc.Interfaces;

namespace HealthTracker.Mvc.Models
{
    public class BloodPressureViewModel : IMeasurementPersonViewModel
    {
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public BloodPressureMeasurement Measurement { get; set; } = new();
    }
}
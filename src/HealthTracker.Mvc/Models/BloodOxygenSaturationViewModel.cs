using HealthTracker.Entities.Measurements;

namespace HealthTracker.Mvc.Models
{
    public class BloodOxygenSaturationViewModel : TimestampViewModel
    {
        public BloodOxygenSaturationMeasurement Measurement { get; set; } = new();

        public BloodOxygenSaturationViewModel()
        {
            Measurement.Id = 0;
            Measurement.PersonId = 0;
            Measurement.Date = DateTime.Now;
            Measurement.Percentage = 0;
        }
    }
}
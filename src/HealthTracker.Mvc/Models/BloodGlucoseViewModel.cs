using HealthTracker.Entities.Measurements;

namespace HealthTracker.Mvc.Models
{
    public class BloodGlucoseViewModel : TimestampViewModel
    {
        public BloodGlucoseMeasurement Measurement { get; set; } = new();

        public BloodGlucoseViewModel()
        {
            Measurement.Id = 0;
            Measurement.PersonId = 0;
            Measurement.Date = DateTime.Now;
            Measurement.Level = 0;
        }
    }
}
using HealthTracker.Entities.Measurements;

namespace HealthTracker.Mvc.Models
{
    public class WeightViewModel : TimestampViewModel
    {
        public WeightMeasurement Measurement { get; set; } = new();

        public WeightViewModel()
        {
            Measurement.Id = 0;
            Measurement.PersonId = 0;
            Measurement.Date = DateTime.Now;
            Measurement.Weight = 0;
        }
    }
}
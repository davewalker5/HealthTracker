using HealthTracker.Entities.Measurements;

namespace HealthTracker.Mvc.Models
{
    public class BloodOxygenSaturationViewModel : SelectedFiltersViewModel
    {
        public BloodOxygenSaturationMeasurement Measurement { get; set; } = new();
        public string Action { get; set; }

        public BloodOxygenSaturationViewModel()
        {
            Measurement.Id = 0;
            Measurement.PersonId = 0;
            Measurement.Date = DateTime.Now;
            Measurement.Percentage = 0;
        }
    }
}
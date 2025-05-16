using HealthTracker.Entities.Measurements;

namespace HealthTracker.Mvc.Models
{
    public class BloodPressureViewModel : SelectedFiltersViewModel
    {
        public BloodPressureMeasurement Measurement { get; set; } = new();
        public string Action { get; set; }

        public BloodPressureViewModel()
        {
            Measurement.Id = 0;
            Measurement.PersonId = 0;
            Measurement.Date = DateTime.Now;
            Measurement.Systolic = 0;
            Measurement.Diastolic = 0;
        }
    }
}
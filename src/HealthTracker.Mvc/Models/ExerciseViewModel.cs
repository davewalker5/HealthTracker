using System.ComponentModel;
using HealthTracker.Entities.Measurements;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Models
{
    public class ExerciseViewModel : SelectedFiltersViewModel
    {
        public IList<SelectListItem> ActivityTypes { get; set; } = [];
        public ExerciseMeasurement Measurement { get; set; } = new();
        public string Action { get; set; }

        public ExerciseViewModel()
        {
            Measurement.Id = 0;
            Measurement.PersonId = 0;
            Measurement.ActivityTypeId = 0;
            Measurement.Date = DateTime.Now;
            Measurement.Duration = 0;
            Measurement.Distance = null;
            Measurement.Calories = 0;
            Measurement.MinimumHeartRate = 0;
            Measurement.MaximumHeartRate = 0;
        }
    }
}
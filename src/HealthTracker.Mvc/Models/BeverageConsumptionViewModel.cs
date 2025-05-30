using HealthTracker.Entities.Measurements;
using HealthTracker.Enumerations.Enumerations;
using HealthTracker.Enumerations.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Models
{
    public class BeverageConsumptionViewModel : TimestampViewModel
    {
        public IList<SelectListItem> Beverages { get; set; } = [];
        public IList<SelectListItem> Measures { get; set; } = [];
        public BeverageConsumptionMeasurement Measurement { get; set; } = new();

        public BeverageConsumptionViewModel()
        {
            // Populate the list of measures
            var measures = Enum.GetValues<BeverageMeasure>().OrderBy(x => x.ToName());
            foreach (var measure in measures)
            {
                Measures.Add(new SelectListItem() { Text = measure.ToName(), Value = ((int)measure).ToString() });
            }
        }

        /// <summary>
        /// Create a new, empty measurement for the specified person
        /// </summary>
        /// <param name="personId"></param>
        public void CreateMeasurement(int personId)
            => SetMeasurement(
                new BeverageConsumptionMeasurement()
                {
                    PersonId = personId,
                    Date = DateTime.Now
                });

        /// <summary>
        /// Set the measurement and the date and time strings
        /// </summary>
        /// <param name="measurement"></param>
        public void SetMeasurement(BeverageConsumptionMeasurement measurement)
        {
            Measurement = measurement;
            SetTimestamp(Measurement.Date);
        }
    }
}
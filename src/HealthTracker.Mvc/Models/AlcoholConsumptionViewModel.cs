using HealthTracker.Entities.Measurements;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Models
{
    public class AlcoholConsumptionViewModel : TimestampViewModel
    {
        public IList<SelectListItem> Beverages { get; set; } = [];
        public AlcoholConsumptionMeasurement Measurement { get; set; } = new();

        /// <summary>
        /// Create a new, empty measurement for the specified person
        /// </summary>
        /// <param name="personId"></param>
        public void CreateMeasurement(int personId)
            => SetMeasurement(
                new AlcoholConsumptionMeasurement()
                {
                    PersonId = personId,
                    Date = DateTime.Now
                });

        /// <summary>
        /// Set the measurement and the date and time strings
        /// </summary>
        /// <param name="measurement"></param>
        public void SetMeasurement(AlcoholConsumptionMeasurement measurement)
        {
            Measurement = measurement;
            SetTimestamp(Measurement.Date);
        }
    }
}
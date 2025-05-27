using HealthTracker.Entities.Measurements;

namespace HealthTracker.Mvc.Models
{
    public class BloodOxygenSaturationViewModel : TimestampViewModel
    {
        public BloodOxygenSaturationMeasurement Measurement { get; set; }

        /// <summary>
        /// Create a new, empty measurement for the specified person
        /// </summary>
        /// <param name="personId"></param>
        public void CreateMeasurement(int personId)
            => SetMeasurement(
                new BloodOxygenSaturationMeasurement()
                {
                    PersonId = personId,
                    Date = DateTime.Now
                });

        /// <summary>
        /// Set the measurement and the date and time strings
        /// </summary>
        /// <param name="measurement"></param>
        public void SetMeasurement(BloodOxygenSaturationMeasurement measurement)
        {
            Measurement = measurement;
            SetTimestamp(Measurement.Date);
        }
    }
}
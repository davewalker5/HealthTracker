using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HealthTracker.Mvc.Models
{
    public class TimestampViewModel : SelectedFiltersViewModel
    {
        [DisplayName("Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "You must provide a measurement date")]
        public DateTime MeasurementDate { get; set; }

        [DisplayName("Time")]
        [DataType(DataType.Time)]
        [Required(ErrorMessage = "You must provide a measurement time")]
        public DateTime MeasurementTime { get; set; }

        public string Action { get; set; }

        /// <summary>
        /// Return a timestamp from the combined date and time strings
        /// </summary>
        /// <returns></returns>
        public DateTime Timestamp()
            => MeasurementDate.Date + MeasurementTime.TimeOfDay;

        /// <summary>
        /// Set the date and time strings
        /// </summary>
        /// <param name="date"></param>
        public void SetTimestamp(DateTime date)
        {
            MeasurementDate = date;
            MeasurementTime = new DateTime(
                date.Year,
                date.Month,
                date.Day,
                date.Hour,
                date.Minute,
                0,
                0
            );
        }
    }
}
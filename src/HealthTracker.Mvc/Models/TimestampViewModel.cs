using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using HealthTracker.Mvc.Entities;

namespace HealthTracker.Mvc.Models
{
    public class TimestampViewModel : SelectedFiltersViewModel
    {
        [DisplayName("Date")]
        [Required(ErrorMessage = "You must provide a measurement date")]
        public string MeasurementDate { get; set; }

        [DisplayName("Time")]
        [Required(ErrorMessage = "You must provide a measurement time")]
        public string MeasurementTime { get; set; }

        public string Action { get; set; }

        /// <summary>
        /// Return a timestamp from the combined date and time strings
        /// </summary>
        /// <returns></returns>
        public DateTime Timestamp()
            => DateTime.ParseExact($"{MeasurementDate} {MeasurementTime}", DateFormats.DateTime, CultureInfo.InvariantCulture);

        /// <summary>
        /// Set the date and time strings
        /// </summary>
        /// <param name="date"></param>
        public void SetTimestamp(DateTime date)
        {
            MeasurementDate = date.ToString(DateFormats.Date);
            MeasurementTime = date.ToString(DateFormats.Time);
        }
    }
}
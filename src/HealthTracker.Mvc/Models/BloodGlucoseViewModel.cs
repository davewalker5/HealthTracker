using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using HealthTracker.Entities.Measurements;
using HealthTracker.Mvc.Entities;

namespace HealthTracker.Mvc.Models
{
    public class BloodGlucoseViewModel : SelectedFiltersViewModel
    {
        [DisplayName("Date")]
        [Required(ErrorMessage = "You must provide a measurement date")]
        public string MeasurementDate { get; set; }

        [DisplayName("Time")]
        [Required(ErrorMessage = "You must provide a measurement time")]
        public string MeasurementTime { get; set; }

        public BloodGlucoseMeasurement Measurement { get; set; } = new();
        public string Action { get; set; }

        public BloodGlucoseViewModel()
        {
            Measurement.Id = 0;
            Measurement.PersonId = 0;
            Measurement.Date = DateTime.Now;
            Measurement.Level = 0;
            MeasurementDate = Measurement.Date.ToString(DateFormats.Date);
            MeasurementTime = Measurement.Date.ToString(DateFormats.Time);
        }

        /// <summary>
        /// Return a timestamp from the combined date and time strings
        /// </summary>
        /// <returns></returns>
        public DateTime Timestamp()
            => DateTime.ParseExact($"{MeasurementDate} {MeasurementTime}", DateFormats.DateTime, CultureInfo.InvariantCulture);
    }
}
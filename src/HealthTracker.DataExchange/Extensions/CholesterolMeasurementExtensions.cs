using System.Collections.Generic;
using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Measurements;
using HealthTracker.DataExchange.Entities;

namespace HealthTracker.DataExchange.Extensions
{
    public static class CholesterolMeasurementExtensions
    {
        /// <summary>
        /// Return an exportable cholesterol measurement from a cholesterol measuement
        /// </summary>
        /// <param name="measurement"></param>
        /// <param name="people"></param>
        /// <returns></returns>
        public static ExportableCholesterolMeasurement ToExportable(this CholesterolMeasurement measurement, IEnumerable<Person> people)
        {
            var person = people.First(x => x.Id == measurement.PersonId);
            return new ExportableCholesterolMeasurement
            {
                PersonId = measurement.PersonId,
                Name = $"{person.FirstNames} {person.Surname}",
                Date = measurement.Date,
                Total = measurement.Total,
                HDL = measurement.HDL,
                LDL = measurement.LDL,
                Triglycerides = measurement.Triglycerides
            };
        }

        /// <summary>
        /// Return a collection of exportable cholesterol measurements from a collection of cholesterol measurements
        /// </summary>
        /// <param name="measurements"></param>
        /// <param name="people"></param>
        /// <returns></returns>
        public static IEnumerable<ExportableCholesterolMeasurement> ToExportable(
            this IEnumerable<CholesterolMeasurement> measurements,
            IEnumerable<Person> people)
        {
            var exportable = new List<ExportableCholesterolMeasurement>();

            foreach (var measurement in measurements)
            {
                exportable.Add(measurement.ToExportable(people));
            }

            return exportable;
        }

        /// <summary>
        /// Return a cholesterol measurement from an exportable cholesterol measurement
        /// </summary>
        /// <param name="exportable"></param>
        /// <returns></returns>
        public static CholesterolMeasurement FromExportable(this ExportableCholesterolMeasurement exportable)
            => new CholesterolMeasurement
              {
                    PersonId = exportable.PersonId,
                    Date = exportable.Date,
                    Total = exportable.Total,
                    HDL = exportable.HDL,
                    LDL = exportable.LDL,
                    Triglycerides = exportable.Triglycerides
              };

        /// <summary>
        /// Return a collection of cholesterol measurements from a collection of exportable cholesterol measurements
        /// </summary>
        /// <param name="exportable"></param>
        /// <returns></returns>
        public static IEnumerable<CholesterolMeasurement> FromExportable(this IEnumerable<ExportableCholesterolMeasurement> exportable)
        {
            var measurements = new List<CholesterolMeasurement>();

            foreach (var measurement in exportable)
            {
                measurements.Add(measurement.FromExportable());
            }

            return measurements;
        }
    }
}

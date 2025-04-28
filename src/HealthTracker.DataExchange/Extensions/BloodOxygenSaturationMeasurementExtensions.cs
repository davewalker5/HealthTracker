using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Measurements;
using HealthTracker.DataExchange.Entities;

namespace HealthTracker.DataExchange.Extensions
{
    public static class BloodOxygenSaturationMeasurementExtensions
    {
        /// <summary>
        /// Return an exportable % SPO2 measurement from a % SPO2 measuement
        /// </summary>
        /// <param name="measurement"></param>
        /// <param name="people"></param>
        /// <returns></returns>
        public static ExportableBloodOxygenSaturationMeasurement ToExportable(this BloodOxygenSaturationMeasurement measurement, IEnumerable<Person> people)
        {
            var person = people.First(x => x.Id == measurement.PersonId);
            return new ExportableBloodOxygenSaturationMeasurement
            {
                PersonId = measurement.PersonId,
                Name = $"{person.FirstNames} {person.Surname}",
                Date = measurement.Date,
                Percentage = measurement.Percentage,
                Assessment = measurement.Assessment
            };
        }

        /// <summary>
        /// Return a collection of exportable % SPO2 measurements from a collection of % SPO2 measurements
        /// </summary>
        /// <param name="measurements"></param>
        /// <param name="people"></param>
        /// <returns></returns>
        public static IEnumerable<ExportableBloodOxygenSaturationMeasurement> ToExportable(
            this IEnumerable<BloodOxygenSaturationMeasurement> measurements,
            IEnumerable<Person> people)
        {
            var exportable = new List<ExportableBloodOxygenSaturationMeasurement>();

            foreach (var measurement in measurements)
            {
                exportable.Add(measurement.ToExportable(people));
            }

            return exportable;
        }

        /// <summary>
        /// Return a % SPO2 measurement from an exportable % SPO2 measurement
        /// </summary>
        /// <param name="exportable"></param>
        /// <returns></returns>
        public static BloodOxygenSaturationMeasurement FromExportable(this ExportableBloodOxygenSaturationMeasurement exportable)
            => new BloodOxygenSaturationMeasurement
              {
                    PersonId = exportable.PersonId,
                    Date = exportable.Date,
                    Percentage = exportable.Percentage,
                    Assessment = exportable.Assessment
              };

        /// <summary>
        /// Return a collection of % SPO2 measurements from a collection of exportable % SPO2 measurements
        /// </summary>
        /// <param name="exportable"></param>
        /// <returns></returns>
        public static IEnumerable<BloodOxygenSaturationMeasurement> FromExportable(this IEnumerable<ExportableBloodOxygenSaturationMeasurement> exportable)
        {
            var measurements = new List<BloodOxygenSaturationMeasurement>();

            foreach (var measurement in exportable)
            {
                measurements.Add(measurement.FromExportable());
            }

            return measurements;
        }
    }
}

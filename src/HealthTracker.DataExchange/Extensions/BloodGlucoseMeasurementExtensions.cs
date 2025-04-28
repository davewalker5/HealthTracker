using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Measurements;
using HealthTracker.DataExchange.Entities;

namespace HealthTracker.DataExchange.Extensions
{
    public static class BloodGlucoseMeasurementExtensions
    {
        /// <summary>
        /// Return an exportable blood glucose measurement from a blood glucose measuement
        /// </summary>
        /// <param name="measurement"></param>
        /// <param name="people"></param>
        /// <returns></returns>
        public static ExportableBloodGlucoseMeasurement ToExportable(this BloodGlucoseMeasurement measurement, IEnumerable<Person> people)
        {
            var person = people.First(x => x.Id == measurement.PersonId);
            return new ExportableBloodGlucoseMeasurement
            {
                PersonId = measurement.PersonId,
                Name = $"{person.FirstNames} {person.Surname}",
                Date = measurement.Date,
                Level = measurement.Level
            };
        }

        /// <summary>
        /// Return a collection of exportable blood glucose measurements from a collection of blood glucose measurements
        /// </summary>
        /// <param name="measurements"></param>
        /// <param name="people"></param>
        /// <returns></returns>
        public static IEnumerable<ExportableBloodGlucoseMeasurement> ToExportable(
            this IEnumerable<BloodGlucoseMeasurement> measurements,
            IEnumerable<Person> people)
        {
            var exportable = new List<ExportableBloodGlucoseMeasurement>();

            foreach (var measurement in measurements)
            {
                exportable.Add(measurement.ToExportable(people));
            }

            return exportable;
        }

        /// <summary>
        /// Return a blood glucose measurement from an exportable blood glucose measurement
        /// </summary>
        /// <param name="exportable"></param>
        /// <returns></returns>
        public static BloodGlucoseMeasurement FromExportable(this ExportableBloodGlucoseMeasurement exportable)
            => new BloodGlucoseMeasurement
              {
                    PersonId = exportable.PersonId,
                    Date = exportable.Date,
                    Level = exportable.Level
              };

        /// <summary>
        /// Return a collection of blood glucose measurements from a collection of exportable blood glucose measurements
        /// </summary>
        /// <param name="exportable"></param>
        /// <returns></returns>
        public static IEnumerable<BloodGlucoseMeasurement> FromExportable(this IEnumerable<ExportableBloodGlucoseMeasurement> exportable)
        {
            var measurements = new List<BloodGlucoseMeasurement>();

            foreach (var measurement in exportable)
            {
                measurements.Add(measurement.FromExportable());
            }

            return measurements;
        }
    }
}

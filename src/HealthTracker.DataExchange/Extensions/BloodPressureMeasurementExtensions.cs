using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Measurements;
using HealthTracker.DataExchange.Entities;

namespace HealthTracker.DataExchange.Extensions
{
    public static class BloodPressureMeasurementExtensions
    {
        /// <summary>
        /// Return an exportable blood pressure measurement from a blood pressure measuement
        /// </summary>
        /// <param name="measurement"></param>
        /// <param name="people"></param>
        /// <returns></returns>
        public static ExportableBloodPressureMeasurement ToExportable(this BloodPressureMeasurement measurement, IEnumerable<Person> people)
        {
            var person = people.First(x => x.Id == measurement.PersonId);
            return new ExportableBloodPressureMeasurement
            {
                PersonId = measurement.PersonId,
                Name = $"{person.FirstNames} {person.Surname}",
                Date = measurement.Date,
                Systolic = measurement.Systolic,
                Diastolic = measurement.Diastolic,
                Assessment = measurement.Assessment
            };
        }

        /// <summary>
        /// Return a collection of exportable blood pressure measurements from a collection of blood pressure measurements
        /// </summary>
        /// <param name="measurements"></param>
        /// <param name="people"></param>
        /// <returns></returns>
        public static IEnumerable<ExportableBloodPressureMeasurement> ToExportable(
            this IEnumerable<BloodPressureMeasurement> measurements,
            IEnumerable<Person> people)
        {
            var exportable = new List<ExportableBloodPressureMeasurement>();

            foreach (var measurement in measurements)
            {
                exportable.Add(measurement.ToExportable(people));
            }

            return exportable;
        }

        /// <summary>
        /// Return a blood pressure measurement from an exportable blood pressure measurement
        /// </summary>
        /// <param name="exportable"></param>
        /// <returns></returns>
        public static BloodPressureMeasurement FromExportable(this ExportableBloodPressureMeasurement exportable)
            => new BloodPressureMeasurement
              {
                    PersonId = exportable.PersonId,
                    Date = exportable.Date,
                    Systolic = exportable.Systolic,
                    Diastolic = exportable.Diastolic,
                    Assessment = exportable.Assessment
              };

        /// <summary>
        /// Return a collection of blood pressure measurements from a collection of exportable blood pressure measurements
        /// </summary>
        /// <param name="exportable"></param>
        /// <returns></returns>
        public static IEnumerable<BloodPressureMeasurement> FromExportable(this IEnumerable<ExportableBloodPressureMeasurement> exportable)
        {
            var measurements = new List<BloodPressureMeasurement>();

            foreach (var measurement in exportable)
            {
                measurements.Add(measurement.FromExportable());
            }

            return measurements;
        }
    }
}

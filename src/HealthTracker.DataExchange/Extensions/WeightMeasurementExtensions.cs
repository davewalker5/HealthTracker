using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Measurements;
using HealthTracker.DataExchange.Entities;

namespace HealthTracker.DataExchange.Extensions
{
    public static class WeightMeasurementExtensions
    {
        /// <summary>
        /// Return an exportable weight measurement from a weight measuement
        /// </summary>
        /// <param name="measurement"></param>
        /// <param name="people"></param>
        /// <returns></returns>
        public static ExportableWeightMeasurement ToExportable(this WeightMeasurement measurement, IEnumerable<Person> people)
        {
            var person = people.First(x => x.Id == measurement.PersonId);
            return new ExportableWeightMeasurement
            {
                PersonId = measurement.PersonId,
                Name = $"{person.FirstNames} {person.Surname}",
                Date = measurement.Date,
                Weight = measurement.Weight,
                BMI = measurement.BMI,
                BMIAssessment = measurement.BMIAssessment
            };
        }

        /// <summary>
        /// Return a collection of exportable weight measurements from a collection of weight measurements
        /// </summary>
        /// <param name="measurements"></param>
        /// <param name="people"></param>
        /// <returns></returns>
        public static IEnumerable<ExportableWeightMeasurement> ToExportable(
            this IEnumerable<WeightMeasurement> measurements,
            IEnumerable<Person> people)
        {
            var exportable = new List<ExportableWeightMeasurement>();

            foreach (var measurement in measurements)
            {
                exportable.Add(measurement.ToExportable(people));
            }

            return exportable;
        }

        /// <summary>
        /// Return a weight measurement from an exportable weight measurement
        /// </summary>
        /// <param name="exportable"></param>
        /// <returns></returns>
        public static WeightMeasurement FromExportable(this ExportableWeightMeasurement exportable)
            => new WeightMeasurement
              {
                    PersonId = exportable.PersonId,
                    Date = exportable.Date,
                    Weight = exportable.Weight,
              };

        /// <summary>
        /// Return a collection of weight measurements from a collection of exportable weight measurements
        /// </summary>
        /// <param name="exportable"></param>
        /// <returns></returns>
        public static IEnumerable<WeightMeasurement> FromExportable(this IEnumerable<ExportableWeightMeasurement> exportable)
        {
            var measurements = new List<WeightMeasurement>();

            foreach (var measurement in exportable)
            {
                measurements.Add(measurement.FromExportable());
            }

            return measurements;
        }
    }
}

using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Measurements;
using HealthTracker.DataExchange.Entities;
using HealthTracker.Logic.Extensions;

namespace HealthTracker.DataExchange.Extensions
{
    public static class ExerciseMeasurementExtensions
    {
        /// <summary>
        /// Return an exportable exercise measurement from an exercise measuement
        /// </summary>
        /// <param name="measurement"></param>
        /// <param name="people"></param>
        /// <param name="activityTypes"></param>
        /// <returns></returns>
        public static ExportableExerciseMeasurement ToExportable(this ExerciseMeasurement measurement, IEnumerable<Person> people, IEnumerable<ActivityType> activityTypes)
        {
            var person = people.First(x => x.Id == measurement.PersonId);
            var activityType = activityTypes.First(x => x.Id == measurement.ActivityTypeId);
            return new ExportableExerciseMeasurement
            {
                PersonId = measurement.PersonId,
                Name = $"{person.FirstNames} {person.Surname}",
                Date = measurement.Date,
                ActivityType = activityType.Description,
                Duration = measurement.Duration.ToFormattedDuration(),
                Distance = measurement.Distance ?? 0,
                Calories = measurement.Calories,
                MinimumHeartRate = measurement.MinimumHeartRate,
                MaximumHeartRate = measurement.MaximumHeartRate
            };
        }

        /// <summary>
        /// Return a collection of exportable exercise measurements from a collection of exercise measurements
        /// </summary>
        /// <param name="measurements"></param>
        /// <param name="people"></param>
        /// <param name="activityTypes"></param>
        /// <returns></returns>
        public static IEnumerable<ExportableExerciseMeasurement> ToExportable(
            this IEnumerable<ExerciseMeasurement> measurements,
            IEnumerable<Person> people,
            IEnumerable<ActivityType> activityTypes)
        {
            var exportable = new List<ExportableExerciseMeasurement>();

            foreach (var measurement in measurements)
            {
                exportable.Add(measurement.ToExportable(people, activityTypes));
            }

            return exportable;
        }

        /// <summary>
        /// Return a exercise measurement from an exportable exercise measurement
        /// </summary>
        /// <param name="exportable"></param>
        /// <param name="activityTypes"></param>
        /// <returns></returns>
        public static ExerciseMeasurement FromExportable(this ExportableExerciseMeasurement exportable, IEnumerable<ActivityType> activityTypes)
        {
            var activityType = activityTypes.First(x => x.Description.Equals(exportable.ActivityType, StringComparison.OrdinalIgnoreCase));
            return new ExerciseMeasurement
            {
                PersonId = exportable.PersonId,
                Date = exportable.Date,
                ActivityTypeId = activityType.Id,
                Duration = exportable.Duration.ToDuration(),
                Distance = exportable.Distance > 0 ? exportable.Distance : null,
                Calories = exportable.Calories,
                MinimumHeartRate = exportable.MinimumHeartRate,
                MaximumHeartRate = exportable.MaximumHeartRate
            };
        }

        /// <summary>
        /// Return a collection of exercise measurements from a collection of exportable exercise measurements
        /// </summary>
        /// <param name="exportable"></param>
        /// <param name="activityTypes"></param>
        /// <returns></returns>
        public static IEnumerable<ExerciseMeasurement> FromExportable(
            this IEnumerable<ExportableExerciseMeasurement> exportable,
            IEnumerable<ActivityType> activityTypes)
        {
            var measurements = new List<ExerciseMeasurement>();

            foreach (var measurement in exportable)
            {
                measurements.Add(measurement.FromExportable(activityTypes));
            }

            return measurements;
        }
    }
}

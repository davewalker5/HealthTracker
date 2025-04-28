using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Logging;
using HealthTracker.Entities.Measurements;
using HealthTracker.Logic.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HealthTracker.Logic.Database
{
    public class ExerciseMeasurementManager : MeasurementManagerBase, IExerciseMeasurementManager
    {
        internal ExerciseMeasurementManager(IHealthTrackerFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return all measurements matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<List<ExerciseMeasurement>> ListAsync(Expression<Func<ExerciseMeasurement, bool>> predicate)
            => await Context.ExerciseMeasurements
                            .Where(predicate)
                            .OrderBy(x => x.Date)
                            .ToListAsync();

        /// <summary>
        /// Add a new measurement
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="activityTypeId"></param>
        /// <param name="date"></param>
        /// <param name="duration"></param>
        /// <param name="distance"></param>
        /// <param name="calories"></param>
        /// <param name="minimumHeartRate"></param>
        /// <param name="maximumHeartRate"></param>
        /// <returns></returns>
        public async Task<ExerciseMeasurement> AddAsync(
            int personId,
            int activityTypeId,
            DateTime date,
            int duration,
            decimal? distance,
            int calories,
            int minimumHeartRate,
            int maximumHeartRate)
        {
            var logDistance = distance == null ? "null" : distance.ToString();
            Factory.Logger.LogMessage(Severity.Info, $"Adding exercise measurement: Person ID {personId}, {date.ToShortDateString()}, Duration {duration.ToFormattedDuration()}, Distance {logDistance}, Calories {calories}, Heart Rate {minimumHeartRate} to {maximumHeartRate}");

            CheckPersonExists(personId);
            CheckActivityTypeExists(activityTypeId);

            var measurement = new ExerciseMeasurement
            {
                PersonId = personId,
                ActivityTypeId = activityTypeId,
                Date = date,
                Duration = duration,
                Distance = distance,
                Calories = calories,
                MinimumHeartRate = minimumHeartRate,
                MaximumHeartRate = maximumHeartRate
            };

            await Context.ExerciseMeasurements.AddAsync(measurement);
            await Context.SaveChangesAsync();

            return measurement;
        }

        /// <summary>
        /// Update the properties of the specified measurement
        /// </summary>
        /// <param name="id"></param>
        /// <param name="personId"></param>
        /// <param name="activityTypeId"></param>
        /// <param name="date"></param>
        /// <param name="duration"></param>
        /// <param name="distance"></param>
        /// <param name="calories"></param>
        /// <param name="minimumHeartRate"></param>
        /// <param name="maximumHeartRate"></param>
        /// <returns></returns>
        public async Task<ExerciseMeasurement> UpdateAsync(
            int id,
            int personId,
            int activityTypeId,
            DateTime date,
            int duration,
            decimal? distance,
            int calories,
            int minimumHeartRate,
            int maximumHeartRate)
        {
            var logDistance = distance == null ? "null" : distance.ToString();
            Factory.Logger.LogMessage(Severity.Info, $"Updating exercise measurement with ID {id}: Person ID {personId}, {date.ToShortDateString()}, Duration {duration.ToFormattedDuration()}, Distance {logDistance}, Calories {calories}, Heart Rate {minimumHeartRate} to {maximumHeartRate}");

            var measurement = Context.ExerciseMeasurements.FirstOrDefault(x => x.Id == id);
            if (measurement != null)
            {
                CheckPersonExists(personId);
                CheckActivityTypeExists(activityTypeId);

                // Save the changes
                measurement.PersonId = personId;
                measurement.ActivityTypeId = activityTypeId;
                measurement.Date = date;
                measurement.Duration = duration;
                measurement.Distance = distance;
                measurement.Calories = calories;
                measurement.MinimumHeartRate = minimumHeartRate;
                measurement.MaximumHeartRate = maximumHeartRate;
                await Context.SaveChangesAsync();
            }

            return measurement;
        }

        /// <summary>
        /// Delete the measurement with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Deleting exercise measurement with ID {id}");

            var measurement = Context.ExerciseMeasurements.FirstOrDefault(x => x.Id == id);
            if (measurement != null)
            {
                Factory.Context.Remove(measurement);
                await Factory.Context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Check an activity type with a specified ID exists and raise an exception if not
        /// </summary>
        /// <param name="activityTypeId"></param>
        private void CheckActivityTypeExists(int activityTypeId)
        {
            var activityType = Context.ActivityTypes.FirstOrDefault(x => x.Id == activityTypeId);
            if (activityType == null)
            {
                var message = $"Activity type with Id {activityTypeId} does not exist";
                throw new ActivityTypeNotFoundException(message);
            }
        }
    }
}
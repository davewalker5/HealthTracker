using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Logging;
using HealthTracker.Entities.Measurements;
using HealthTracker.Logic.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HealthTracker.Logic.Database
{
    public class ActivityTypeManager : DatabaseManagerBase, IActivityTypeManager
    {
        internal ActivityTypeManager(IHealthTrackerFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return the first activity type matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<ActivityType> GetAsync(Expression<Func<ActivityType, bool>> predicate)
        {
            var activityTypes = await ListAsync(predicate, 1, int.MaxValue);
            return activityTypes.FirstOrDefault();
        }

        /// <summary>
        /// Return all activity types matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<ActivityType>> ListAsync(Expression<Func<ActivityType, bool>> predicate, int pageNumber, int pageSize)
            => await Context.ActivityTypes
                            .Where(predicate)
                            .OrderBy(x => x.Description)
                            .ToListAsync();

        /// <summary>
        /// Add an activity type, if it doesn't already exist
        /// </summary>
        /// <param name="firstnames"></param>
        /// <param name="surname"></param>
        /// <param name="dob"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public async Task<ActivityType> AddAsync(string description)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Creating new activity type '{description}'");

            var clean = StringCleaner.Clean(description);
            await CheckActivityTypeIsNotADuplicate(clean);

            var activityType = new ActivityType
            {
                Description = clean
            };

            await Context.ActivityTypes.AddAsync(activityType);
            await Context.SaveChangesAsync();

            return activityType;
        }


        /// <summary>
        /// Update the properties of the specified person
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public async Task<ActivityType> UpdateAsync(int id, string description)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Updating activity type with ID {id} to '{description}'");

            var clean = StringCleaner.Clean(description);
            await CheckActivityTypeIsNotADuplicate(clean);

            var activityType = Context.ActivityTypes.FirstOrDefault(x => x.Id == id);
            if (activityType != null)
            {
                // Save the changes
                activityType.Description = clean;
                await Context.SaveChangesAsync();
            }

            return activityType;
        }

        /// <summary>
        /// Delete the person with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Deleting activity type with ID {id}");

            var activityType = await GetAsync(x => x.Id == id);
            if (activityType != null)
            {
                // Check the activity type isn't in use
                var measurement = Context.ExerciseMeasurements.FirstOrDefault(x => x.ActivityTypeId == id);
                if (measurement != null)
                {
                    var message = $"Activity type with Id {id} has measurements associated with it and cannot be deleted";
                    throw new ActivityTypeInUseException(message);
                }

                // Delete the activity type record and save changes
                Factory.Context.Remove(activityType);
                await Factory.Context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Raise an exception if an attempt is made to add/update an activity type with a duplicate
        /// description
        /// </summary>
        /// <param name="description"></param>
        /// <exception cref="ActivityTypeInUseException"></exception>
        private async Task CheckActivityTypeIsNotADuplicate(string description)
        {
            var activityType = await Context.ActivityTypes.FirstOrDefaultAsync(x => x.Description == description);
            if (activityType != null)
            {
                var message = $"Activity type {description} already exists";
                throw new ActivityTypeExistsException(message);
            }
        }
    }
}

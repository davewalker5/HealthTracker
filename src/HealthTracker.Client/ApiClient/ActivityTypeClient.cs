using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Measurements;
using Microsoft.Extensions.Logging;

namespace HealthTracker.Client.ApiClient
{
    public class ActivityTypeClient : HealthTrackerClientBase, IActivityTypeClient
    {
        private const string RouteKey = "ActivityType";

        public ActivityTypeClient(
            IHealthTrackerHttpClient client,
            IHealthTrackerApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ILogger<ActivityTypeClient> logger)
            : base(client, settings, tokenProvider, logger)
        {
        }

        /// <summary>
        /// Add a new activity type to the database
        /// </summary>
        /// <param
        /// <param name="description"></param>
        /// <param name="distanceBased"></param>
        /// <returns></returns>
        public async Task<ActivityType> AddAsync(string description, bool distanceBased)
        {
            dynamic template = new
            {
                Description = description,
                DistanceBased = distanceBased
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            var activityType = Deserialize<ActivityType>(json);

            return activityType;
        }

        /// <summary>
        /// Update an existing activity types details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        /// <param name="distanceBased"></param>
        /// <returns></returns>
        public async Task<ActivityType> UpdateAsync(int id, string description, bool distanceBased)
        {
            dynamic template = new
            {
                Id = id,
                Description = description,
                DistanceBased = distanceBased
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            var activityType = Deserialize<ActivityType>(json);

            return activityType;
        }

        /// <summary>
        /// Delete an activity type from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            var route = $"{baseRoute}/{id}";
            _ = await SendDirectAsync(route, null, HttpMethod.Delete);
        }

        /// <summary>
        /// Return a list of activity types
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<ActivityType>> ListAsync(int pageNumber, int pageSize)
        {
            // Request a list of activity types
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            var route = $"{baseRoute}/{pageNumber}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no activity types in the database
            List<ActivityType> activityTypes = Deserialize<List<ActivityType>>(json);
            return activityTypes;
        }
    }
}

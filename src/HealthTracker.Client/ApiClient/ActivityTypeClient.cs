using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Measurements;

namespace HealthTracker.Client.ApiClient
{
    public class ActivityTypeClient : HealthTrackerClientBase, IActivityTypeClient
    {
        private const string RouteKey = "ActivityType";

        public ActivityTypeClient(IHealthTrackerHttpClient client, IHealthTrackerApplicationSettings settings, IAuthenticationTokenProvider tokenProvider)
            : base(client, settings, tokenProvider)
        {
        }

        /// <summary>
        /// Add a new activity type to the database
        /// </summary>
        /// <param
        /// <returns></returns>
        public async Task<ActivityType> AddActivityTypeAsync(string description)
        {
            dynamic template = new
            {
                Description = description
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
        /// <returns></returns>
        public async Task<ActivityType> UpdateActivityTypeAsync(int id, string description)
        {
            dynamic template = new
            {
                Id = id,
                Description = description
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
        public async Task DeleteActivityTypeAsync(int id)
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
        public async Task<List<ActivityType>> ListActivityTypesAsync(int pageNumber, int pageSize)
        {
            // Request a list of activity types
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            var route = $"{baseRoute}/{pageNumber}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no activity types in the database
            List<ActivityType> activityTypes = !string.IsNullOrEmpty(json) ? Deserialize<List<ActivityType>>(json) : null;
            return activityTypes;
        }
    }
}

using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Measurements;
using Microsoft.Extensions.Logging;

namespace HealthTracker.Client.ApiClient
{
    public class BeverageMeasureClient : HealthTrackerClientBase, IBeverageMeasureClient
    {
        private const string RouteKey = "BeverageMeasure";

        public BeverageMeasureClient(
            IHealthTrackerHttpClient client,
            IHealthTrackerApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ILogger<BeverageMeasureClient> logger)
            : base(client, settings, tokenProvider, logger)
        {
        }

        /// <summary>
        /// Add a new beverage measure to the database
        /// </summary>
        /// <param
        /// <param name="name"></param>
        /// <param name="volume"></param>
        /// <returns></returns>
        public async Task<BeverageMeasure> AddAsync(string name, decimal volume)
        {
            dynamic template = new
            {
                Name = name,
                Volume = volume
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            var beverageMeasure = Deserialize<BeverageMeasure>(json);

            return beverageMeasure;
        }

        /// <summary>
        /// Update an existing beverage measures details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="volume"></param>
        /// <returns></returns>
        public async Task<BeverageMeasure> UpdateAsync(int id, string name, decimal volume)
        {
            dynamic template = new
            {
                Id = id,
                Name = name,
                Volume = volume
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            var beverageMeasure = Deserialize<BeverageMeasure>(json);

            return beverageMeasure;
        }

        /// <summary>
        /// Delete an beverage measure from the database
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
        /// Return a list of beverage measures
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<BeverageMeasure>> ListAsync(int pageNumber, int pageSize)
        {
            // Request a list of beverage measures
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            var route = $"{baseRoute}/{pageNumber}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no beverage measures in the database
            List<BeverageMeasure> beverageMeasures = !string.IsNullOrEmpty(json) ? Deserialize<List<BeverageMeasure>>(json) : null;
            return beverageMeasures;
        }
    }
}

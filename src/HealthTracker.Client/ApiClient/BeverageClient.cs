using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Food;
using Microsoft.Extensions.Logging;

namespace HealthTracker.Client.ApiClient
{
    public class BeverageClient : HealthTrackerClientBase, IBeverageClient
    {
        private const string RouteKey = "Beverage";

        public BeverageClient(
            IHealthTrackerHttpClient client,
            IHealthTrackerApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ILogger<BeverageClient> logger)
            : base(client, settings, tokenProvider, logger)
        {
        }

        /// <summary>
        /// Add a new activity type to the database
        /// </summary>
        /// <param
        /// <param name="name"></param>
        /// <param name="typicalABV"></param>
        /// <param name="isHydrating"></param>
        /// <param name="isAlcohol"></param>
        /// <returns></returns>
        public async Task<Beverage> AddAsync(string name, decimal typicalABV, bool isHydrating, bool isAlcohol)
        {
            dynamic template = new
            {
                Name = name,
                TypicalABV = typicalABV,
                IsHydrating = isHydrating,
                IsAlcohol = isAlcohol
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            var beverage = Deserialize<Beverage>(json);

            return beverage;
        }

        /// <summary>
        /// Update an existing activity types details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="typicalABV"></param>
        /// <param name="isHydrating"></param>
        /// <param name="isAlcohol"></param>
        /// <returns></returns>
        public async Task<Beverage> UpdateAsync(int id, string name, decimal typicalABV, bool isHydrating, bool isAlcohol)
        {
            dynamic template = new
            {
                Id = id,
                Name = name,
                TypicalABV = typicalABV,
                IsHydrating = isHydrating,
                IsAlcohol = isAlcohol
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            var beverage = Deserialize<Beverage>(json);

            return beverage;
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
        public async Task<List<Beverage>> ListAsync(int pageNumber, int pageSize)
        {
            // Request a list of activity types
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            var route = $"{baseRoute}/{pageNumber}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no activity types in the database
            List<Beverage> beverages = Deserialize<List<Beverage>>(json);
            return beverages;
        }
    }
}

using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Food;
using Microsoft.Extensions.Logging;

namespace HealthTracker.Client.ApiClient
{
    public class FoodCategoryClient : HealthTrackerClientBase, IFoodCategoryClient
    {
        private const string RouteKey = "FoodCategory";

        public FoodCategoryClient(
            IHealthTrackerHttpClient client,
            IHealthTrackerApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ILogger<FoodCategoryClient> logger)
            : base(client, settings, tokenProvider, logger)
        {
        }

        /// <summary>
        /// Add a new food category to the database
        /// </summary>
        /// <param
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<FoodCategory> AddAsync(string name)
        {
            dynamic template = new
            {
                Name = name
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            var foodCategory = Deserialize<FoodCategory>(json);

            return foodCategory;
        }

        /// <summary>
        /// Update an existing food category details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<FoodCategory> UpdateAsync(int id, string name)
        {
            dynamic template = new
            {
                Id = id,
                Name = name
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            var foodCategory = Deserialize<FoodCategory>(json);

            return foodCategory;
        }

        /// <summary>
        /// Delete a food category from the database
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
        /// Return a list of food categories
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<FoodCategory>> ListAsync(int pageNumber, int pageSize)
        {
            // Request a list of food categories
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            var route = $"{baseRoute}/{pageNumber}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no food sources in the database
            List<FoodCategory> foodCategories = Deserialize<List<FoodCategory>>(json);
            return foodCategories;
        }
    }
}

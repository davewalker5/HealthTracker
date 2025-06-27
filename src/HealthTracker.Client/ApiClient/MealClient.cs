using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Food;
using Microsoft.Extensions.Logging;

namespace HealthTracker.Client.ApiClient
{
    public class MealClient : HealthTrackerClientBase, IMealClient
    {
        private const string RouteKey = "Meal";
        private const string ExportRouteKey = "ExportMeal";
        private const string ImportRouteKey = "ImportMeal";

        public MealClient(
            IHealthTrackerHttpClient client,
            IHealthTrackerApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ILogger<MealClient> logger)
            : base(client, settings, tokenProvider, logger)
        {
        }

        /// <summary>
        /// Add a new meal
        /// </summary>
        /// <param name="name"></param>
        /// <param name="portion"></param>
        /// <param name="foodSourceId"></param>
        /// <param name="nutritionalValueId"></param>
        /// <returns></returns>
        public async Task<Meal> AddAsync(
            string name,
            decimal portion,
            int foodSourceId,
            int? nutritionalValueId)
        {
            dynamic template = new
            {
                Name = name,
                Portion = portion,
                FoodSourceId = foodSourceId,
                NutritionalValueId = nutritionalValueId
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            var measurement = Deserialize<Meal>(json);

            return measurement;
        }

        /// <summary>
        /// Update an existing meal
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="portion"></param>
        /// <param name="foodSourceId"></param>
        /// <param name="nutritionalValueId"></param>
        /// <returns></returns>
        public async Task<Meal> UpdateAsync(
            int id,
            string name,
            decimal portion,
            int foodSourceId,
            int? nutritionalValueId)
        {
            dynamic template = new
            {
                Id = id,
                Name = name,
                Portion = portion,
                FoodSourceId = foodSourceId,
                NutritionalValueId = nutritionalValueId
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            var measurement = Deserialize<Meal>(json);

            return measurement;
        }

        /// <summary>
        /// Request an import of meals from the content of a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task ImportFromFileContentAsync(string content)
        {
            dynamic data = new{ Content = content };
            var json = Serialize(data);
            await SendIndirectAsync(ImportRouteKey, json, HttpMethod.Post);
        }

        /// <summary>
        /// Request an import of meals given the path to a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task ImportFromFileAsync(string filePath)
            => await ImportFromFileContentAsync(File.ReadAllText(filePath));

        /// <summary>
        /// Request an export of meals to a named file in the API export folder
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task ExportAsync(string fileName)
        {
            dynamic data = new { FileName = fileName };
            var json = Serialize(data);
            await SendIndirectAsync(ExportRouteKey, json, HttpMethod.Post);
        }
        
        /// <summary>
        /// Retrieve a single meal given its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Meal> GetAsync(int id)
        {
            // Request the meal with the specified ID
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            string route = $"{baseRoute}/{id}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // Extract the item from the response
            var item = Deserialize<Meal>(json);
            return item;
        }

        /// <summary>
        /// Delete a meal from the database
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
        /// Return a list of meals
        /// </summary>
        /// <param name="foodSourceId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Meal>> ListAsync(int foodSourceId, int pageNumber, int pageSize)
        {
            // Request a list of meals
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            string route = $"{baseRoute}/{foodSourceId}/{pageNumber}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no items in the database
            List<Meal> items = !string.IsNullOrEmpty(json) ? Deserialize<List<Meal>>(json) : null;
            return items;
        }
    }
}

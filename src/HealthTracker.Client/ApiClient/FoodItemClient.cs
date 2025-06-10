using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Food;
using Microsoft.Extensions.Logging;

namespace HealthTracker.Client.ApiClient
{
    public class FoodItemClient : HealthTrackerClientBase, IFoodItemClient
    {
        private const string RouteKey = "FoodItem";
        private const string ExportRouteKey = "ExportFoodItem";
        private const string ImportRouteKey = "ImportFoodItem";

        public FoodItemClient(
            IHealthTrackerHttpClient client,
            IHealthTrackerApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ILogger<FoodItemClient> logger)
            : base(client, settings, tokenProvider, logger)
        {
        }

        /// <summary>
        /// Add a new food item
        /// </summary>
        /// <param name="name"></param>
        /// <param name="portion"></param>
        /// <param name="foodCategoryId"></param>
        /// <param name="nutritionalValueId"></param>
        /// <returns></returns>
        public async Task<FoodItem> AddAsync(
            string name,
            decimal portion,
            int foodCategoryId,
            int? nutritionalValueId)
        {
            dynamic template = new
            {
                Name = name,
                Portion = portion,
                FoodCategoryId = foodCategoryId,
                NutritionalValueId = nutritionalValueId
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            var measurement = Deserialize<FoodItem>(json);

            return measurement;
        }

        /// <summary>
        /// Update an existing food item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="portion"></param>
        /// <param name="foodCategoryId"></param>
        /// <param name="nutritionalValueId"></param>
        /// <returns></returns>
        public async Task<FoodItem> UpdateAsync(
            int id,
            string name,
            decimal portion,
            int foodCategoryId,
            int? nutritionalValueId)
        {
            dynamic template = new
            {
                Id = id,
                Name = name,
                Portion = portion,
                FoodCategoryId = foodCategoryId,
                NutritionalValueId = nutritionalValueId
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            var measurement = Deserialize<FoodItem>(json);

            return measurement;
        }

        /// <summary>
        /// Request an import of food items from the content of a file
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
        /// Request an import of food items given the path to a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task ImportFromFileAsync(string filePath)
            => await ImportFromFileContentAsync(File.ReadAllText(filePath));

        /// <summary>
        /// Request an export of food items to a named file in the API export folder
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
        /// Retrieve a single food item given its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<FoodItem> GetAsync(int id)
        {
            // Request the food item with the specified ID
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            string route = $"{baseRoute}/{id}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // Extract the item from the response
            var item = Deserialize<FoodItem>(json);
            return item;
        }

        /// <summary>
        /// Delete a food item from the database
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
        /// Return a list of food items
        /// </summary>
        /// <param name="foodCategoryId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<FoodItem>> ListAsync(int foodCategoryId, int pageNumber, int pageSize)
        {
            // Request a list of food items
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            string route = $"{baseRoute}/{foodCategoryId}/{pageNumber}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no items in the database
            List<FoodItem> items = !string.IsNullOrEmpty(json) ? Deserialize<List<FoodItem>>(json) : null;
            return items;
        }
    }
}

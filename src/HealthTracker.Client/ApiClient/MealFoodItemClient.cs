using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Food;
using Microsoft.Extensions.Logging;

namespace HealthTracker.Client.ApiClient
{
    public class MealFoodItemClient : HealthTrackerClientBase, IMealFoodItemClient
    {
        private const string RouteKey = "MealFoodItem";
        private const string ExportRouteKey = "ExportMealFoodItem";
        private const string ImportRouteKey = "ImportMealFoodItem";

        public MealFoodItemClient(
            IHealthTrackerHttpClient client,
            IHealthTrackerApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ILogger<MealFoodItemClient> logger)
            : base(client, settings, tokenProvider, logger)
        {
        }

        /// <summary>
        /// Add a new meal/food item relationship
        /// </summary>
        /// <param name="mealId"></param>
        /// <param name="foodItemId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public async Task<MealFoodItem> AddAsync(int mealId, int foodItemId, decimal quantity)
        {
            dynamic template = new
            {
                MealId = mealId,
                FoodItemId = foodItemId,
                Quantity = quantity
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            var measurement = Deserialize<MealFoodItem>(json);

            return measurement;
        }

        /// <summary>
        /// Update an existing meal/food item relationship
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mealId"></param>
        /// <param name="foodItemId"></param>
        /// <param name="quantity"></param>
        /// <param name="nutritionalValueId"></param>
        /// <returns></returns>
        public async Task<MealFoodItem> UpdateAsync(int id, int mealId, int foodItemId, decimal quantity)
        {
            dynamic template = new
            {
                Id = id,
                MealId = mealId,
                FoodItemId = foodItemId,
                Quantity = quantity
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            var measurement = Deserialize<MealFoodItem>(json);

            return measurement;
        }

        /// <summary>
        /// Request an import of meal/food item relationships from the content of a file
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
        /// Request an import of meal/food item relationships given the path to a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task ImportFromFileAsync(string filePath)
            => await ImportFromFileContentAsync(File.ReadAllText(filePath));

        /// <summary>
        /// Request an export of meal/food item relationships to a named file in the API export folder
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
        /// Retrieve a single meal/food item relationship given its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<MealFoodItem> GetAsync(int id)
        {
            // Request the meal with the specified ID
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            string route = $"{baseRoute}/{id}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // Extract the item from the response
            var item = Deserialize<MealFoodItem>(json);
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
        /// Return a list of meal/food item relationships for a specified meal
        /// </summary>
        /// <param name="mealId"></param>
        /// <returns></returns>
        public async Task<List<MealFoodItem>> ListAsync(int mealId)
        {
            // Request a list of meals
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            string route = $"{baseRoute}/list/{mealId}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no items in the database
            List<MealFoodItem> items = Deserialize<List<MealFoodItem>>(json);
            return items;
        }
    }
}

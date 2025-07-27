using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Food;
using HealthTracker.Enumerations.Enumerations;
using Microsoft.Extensions.Logging;

namespace HealthTracker.Client.ApiClient
{
    public class PlannedMealClient : HealthTrackerClientBase, IPlannedMealClient
    {
        private const string RouteKey = "PlannedMeal";
        private const string ExportRouteKey = "ExportPlannedMeal";
        private const string ImportRouteKey = "ImportPlannedMeal";

        public PlannedMealClient(
            IHealthTrackerHttpClient client,
            IHealthTrackerApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ILogger<PlannedMealClient> logger)
            : base(client, settings, tokenProvider, logger)
        {
        }

        /// <summary>
        /// Add a new planned meal
        /// </summary>
        /// <param name="mealType"></param>
        /// <param name="date"></param>
        /// <param name="mealId"></param>
        /// <returns></returns>
        public async Task<PlannedMeal> AddAsync(MealType mealType, DateTime date, int mealId)
        {
            dynamic template = new
            {
                MealType = mealType,
                Date = date,
                MealId = mealId
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            var measurement = Deserialize<PlannedMeal>(json);

            return measurement;
        }

        /// <summary>
        /// Update an existing planned meal
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mealType"></param>
        /// <param name="date"></param>
        /// <param name="mealId"></param>
        /// <returns></returns>
        public async Task<PlannedMeal> UpdateAsync(int id, MealType mealType, DateTime date, int mealId)
        {
            dynamic template = new
            {
                Id = id,
                MealType = mealType,
                Date = date,
                MealId = mealId
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            var measurement = Deserialize<PlannedMeal>(json);

            return measurement;
        }

        /// <summary>
        /// Request an import of planned meals from the content of a file
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
        /// Request an export of planned meals to a named file in the API export folder
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
        /// Retrieve a single planned meal given its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PlannedMeal> GetAsync(int id)
        {
            // Request the meal with the specified ID
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            string route = $"{baseRoute}/{id}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // Extract the item from the response
            var item = Deserialize<PlannedMeal>(json);
            return item;
        }

        /// <summary>
        /// Delete a planned meal from the database
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
        /// Purge planned meals with a date less than a specified cutoff
        /// </summary>
        /// <param name="cutoff"></param>
        /// <returns></returns>
        public async Task PurgeAsync(DateTime? cutoff)
        {
            dynamic data = new { Cutoff = cutoff };
            var json = Serialize(data);
            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            var route = $"{baseRoute}/purge";
            _ = await SendDirectAsync(route, json, HttpMethod.Post);
        }

        /// <summary>
        /// Return a list of planned meals
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<PlannedMeal>> ListAsync(int pageNumber, int pageSize)
        {
            // Request a list of meals
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            string route = $"{baseRoute}/{pageNumber}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no items in the database
            List<PlannedMeal> items = Deserialize<List<PlannedMeal>>(json);
            return items;
        }
    }
}

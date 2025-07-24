using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Food;
using Microsoft.Extensions.Logging;

namespace HealthTracker.Client.ApiClient
{
    public class MealConsumptionMeasurementClient : HealthTrackerClientBase, IMealConsumptionMeasurementClient
    {
        private const string RouteKey = "MealConsumptionMeasurement";
        private const string ExportRouteKey = "ExportMealConsumptionMeasurement";
        private const string ImportRouteKey = "ImportMealConsumptionMeasurement";

        public MealConsumptionMeasurementClient(
            IHealthTrackerHttpClient client,
            IHealthTrackerApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ILogger<MealConsumptionMeasurementClient> logger)
            : base(client, settings, tokenProvider, logger)
        {
        }

        /// <summary>
        /// Add a new meal consumption measurement to the database
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="mealId"></param>
        /// <param name="date"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public async Task<MealConsumptionMeasurement> AddAsync(
            int personId,
            int mealId,
            DateTime? date,
            decimal quantity)
        {
            dynamic template = new
            {
                PersonId = personId,
                MealId = mealId,
                Date = date ?? DateTime.Now,
                Quantity = quantity
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            var measurement = Deserialize<MealConsumptionMeasurement>(json);

            return measurement;
        }

        /// <summary>
        /// Update an existing meal consumption measurement
        /// </summary>
        /// <param name="id"></param>
        /// <param name="personId"></param>
        /// <param name="mealId"></param>
        /// <param name="date"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public async Task<MealConsumptionMeasurement> UpdateAsync(
            int id,
            int personId,
            int mealId,
            DateTime? date,
            decimal quantity)
        {
            dynamic template = new
            {
                Id = id,
                PersonId = personId,
                MealId = mealId,
                Date = date ?? DateTime.Now,
                Quantity = quantity
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            var measurement = Deserialize<MealConsumptionMeasurement>(json);

            return measurement;
        }

        /// <summary>
        /// Request an import of measurements from the content of a file
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
        /// Request an import of  measurements given the path to a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task ImportFromFileAsync(string filePath)
            => await ImportFromFileContentAsync(File.ReadAllText(filePath));

        /// <summary>
        /// Request an export of measurements to a named file in the API export folder
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task ExportAsync(int personId, DateTime? from, DateTime? to, string fileName)
        {
            dynamic data = new { PersonId = personId, From = from, To = to, FileName = fileName };
            var json = Serialize(data);
            await SendIndirectAsync(ExportRouteKey, json, HttpMethod.Post);
        }
        
        /// <summary>
        /// Retrieve a single measurement given its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<MealConsumptionMeasurement> GetAsync(int id)
        {
            // Request the measurement with the specified ID
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            string route = $"{baseRoute}/{id}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // Extract the measurement from the response
            var measurement = Deserialize<MealConsumptionMeasurement>(json);
            return measurement;
        }

        /// <summary>
        /// Delete a measurement from the database
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
        /// Return a list of measurements
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<MealConsumptionMeasurement>> ListAsync(int personId, DateTime? from, DateTime? to, int pageNumber, int pageSize)
        {
            // Determine the encoded date range
            (var encodedFromDate, var encodedToDate) = CalculateEncodedDateRange(from, to);

            // Request a list of measurements
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            string route = $"{baseRoute}/{personId}/{encodedFromDate}/{encodedToDate}/{pageNumber}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no measurements in the database
            var measurements = Deserialize<List<MealConsumptionMeasurement>>(json);
            return measurements;
        }
    }
}

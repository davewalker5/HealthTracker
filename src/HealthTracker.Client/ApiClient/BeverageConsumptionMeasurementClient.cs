using System.Web;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Food;
using Microsoft.Extensions.Logging;

namespace HealthTracker.Client.ApiClient
{
    public class BeverageConsumptionMeasurementClient : HealthTrackerClientBase, IBeverageConsumptionMeasurementClient
    {
        private const string RouteKey = "BeverageConsumptionMeasurement";
        private const string ExportRouteKey = "ExportBeverageConsumptionMeasurement";
        private const string ImportRouteKey = "ImportBeverageConsumptionMeasurement";

        public BeverageConsumptionMeasurementClient(
            IHealthTrackerHttpClient client,
            IHealthTrackerApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ILogger<BeverageConsumptionMeasurementClient> logger)
            : base(client, settings, tokenProvider, logger)
        {
        }

        /// <summary>
        /// Add a new beverage consumption measurement to the database
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="beverageId"></param>
        /// <param name="date"></param>
        /// <param name="quantity"></param>
        /// <param name="volume"></param>
        /// <param name="abv"></param>
        /// <returns></returns>
        public async Task<BeverageConsumptionMeasurement> AddAsync(
            int personId,
            int beverageId,
            DateTime? date,
            int quantity,
            decimal volume,
            decimal abv)
        {
            dynamic template = new
            {
                PersonId = personId,
                BeverageId = beverageId,
                Date = date ?? DateTime.Now,
                Quantity = quantity,
                Volume = volume,
                ABV = abv
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            var measurement = Deserialize<BeverageConsumptionMeasurement>(json);

            return measurement;
        }

        /// <summary>
        /// Update an existing beverage consumption measurement
        /// </summary>
        /// <param name="id"></param>
        /// <param name="personId"></param>
        /// <param name="beverageId"></param>
        /// <param name="date"></param>
        /// <param name="quantity"></param>
        /// <param name="volume"></param>
        /// <param name="abv"></param>
        /// <returns></returns>
        public async Task<BeverageConsumptionMeasurement> UpdateAsync(
            int id,
            int personId,
            int beverageId,
            DateTime? date,
            int quantity,
            decimal volume,
            decimal abv)
        {
            dynamic template = new
            {
                Id = id,
                PersonId = personId,
                BeverageId = beverageId,
                Date = date ?? DateTime.Now,
                Quantity = quantity,
                Volume = volume,
                ABV = abv
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            var measurement = Deserialize<BeverageConsumptionMeasurement>(json);

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
        public async Task<BeverageConsumptionMeasurement> GetAsync(int id)
        {
            // Request the measurement with the specified ID
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            string route = $"{baseRoute}/{id}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // Extract the measurement from the response
            var measurement = Deserialize<BeverageConsumptionMeasurement>(json);
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
        public async Task<List<BeverageConsumptionMeasurement>> ListAsync(int personId, DateTime? from, DateTime? to, int pageNumber, int pageSize)
        {
            // Determine the encoded date range
            (var encodedFromDate, var encodedToDate) = CalculateEncodedDateRange(from, to);

            // Request a list of measurements
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            string route = $"{baseRoute}/{personId}/{encodedFromDate}/{encodedToDate}/{pageNumber}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no measurements in the database
            var measurements = Deserialize<List<BeverageConsumptionMeasurement>>(json);
            return measurements;
        }

        /// <summary>
        /// Calculate total hydrating beverage consumption for a person for the last "n" days
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public async Task<BeverageConsumptionSummary> CalculateTotalHydratingAsync(int personId, int days)
        {
            // Calculate an inclusive date range that ensures the whole of the start and end days are covered
            var now = DateTime.Now;
            var from = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).AddDays(-days + 1);
            var to = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);

            return await CalculateTotalHydratingAsync(personId, from, to);
        }

        /// <summary>
        /// Calculate total hydrating beverage consumption for a date range and person
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<BeverageConsumptionSummary> CalculateTotalHydratingAsync(int personId, DateTime from, DateTime to)
        {
            var encodedFromDate = HttpUtility.UrlEncode(from.ToString(DateTimeFormat));
            var encodedToDate = HttpUtility.UrlEncode(to.ToString(DateTimeFormat));

            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            var route = $"{baseRoute}/totalhydrating/{personId}/{encodedFromDate}/{encodedToDate}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            var measurement = Deserialize<BeverageConsumptionSummary>(json);

            return measurement;
        }

        /// <summary>
        /// Calculate daily total hydrating beverage consumption for a date range and person
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<List<BeverageConsumptionMeasurement>> CalculateDailyTotalHydratingAsync(int personId, DateTime from, DateTime to)
        {
            var encodedFromDate = HttpUtility.UrlEncode(from.ToString(DateTimeFormat));
            var encodedToDate = HttpUtility.UrlEncode(to.ToString(DateTimeFormat));

            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            var route = $"{baseRoute}/dailytotalhydrating/{personId}/{encodedFromDate}/{encodedToDate}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            var measurements = Deserialize<List<BeverageConsumptionMeasurement>>(json);

            return measurements;
        }

        /// <summary>
        /// Calculate total alcoholic beverage consumption for a person for the last "n" days
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public async Task<BeverageConsumptionSummary> CalculateTotalAlcoholicAsync(int personId, int days)
        {
            // Calculate an inclusive date range that ensures the whole of the start and end days are covered
            var now = DateTime.Now;
            var from = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).AddDays(-days + 1);
            var to = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);

            return await CalculateTotalAlcoholicAsync(personId, from, to);
        }

        /// <summary>
        /// Calculate total acoholic beverage consumption for a date range and person
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<BeverageConsumptionSummary> CalculateTotalAlcoholicAsync(int personId, DateTime from, DateTime to)
        {
            var encodedFromDate = HttpUtility.UrlEncode(from.ToString(DateTimeFormat));
            var encodedToDate = HttpUtility.UrlEncode(to.ToString(DateTimeFormat));

            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            var route = $"{baseRoute}/totalalcohol/{personId}/{encodedFromDate}/{encodedToDate}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            var measurement = Deserialize<BeverageConsumptionSummary>(json);

            return measurement;
        }

        /// <summary>
        /// Calculate daily total alcoholic beverage consumption for a date range and person
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<List<BeverageConsumptionMeasurement>> CalculateDailyTotalAlcoholicAsync(int personId, DateTime from, DateTime to)
        {
            var encodedFromDate = HttpUtility.UrlEncode(from.ToString(DateTimeFormat));
            var encodedToDate = HttpUtility.UrlEncode(to.ToString(DateTimeFormat));

            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            var route = $"{baseRoute}/dailytotalalcohol/{personId}/{encodedFromDate}/{encodedToDate}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            var measurements = Deserialize<List<BeverageConsumptionMeasurement>>(json);

            return measurements;
        }
    }
}

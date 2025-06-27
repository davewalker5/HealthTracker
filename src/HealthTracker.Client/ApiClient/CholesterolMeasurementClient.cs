using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Measurements;
using Microsoft.Extensions.Logging;

namespace HealthTracker.Client.ApiClient
{
    public class CholesterolMeasurementClient : HealthTrackerClientBase, ICholesterolMeasurementClient
    {
        private const string RouteKey = "CholesterolMeasurement";
        private const string ExportRouteKey = "ExportCholesterolMeasurement";
        private const string ImportRouteKey = "ImportCholesterolMeasurement";

        public CholesterolMeasurementClient(
            IHealthTrackerHttpClient client,
            IHealthTrackerApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ILogger<CholesterolMeasurementClient> logger)
            : base(client, settings, tokenProvider, logger)
        {
        }

        /// <summary>
        /// Add a new cholesterol measurement to the database
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="date"></param>
        /// <param name="total"></param>
        /// <param name="hdl"></param>
        /// <param name="ldl"></param>
        /// <param name="triglycerides"></param>
        /// <returns></returns>
        public async Task<CholesterolMeasurement> AddAsync(
            int personId,
            DateTime? date,
            decimal total,
            decimal hdl,
            decimal ldl,
            decimal triglycerides)
        {
            dynamic template = new
            {
                PersonId = personId,
                Date = date ?? DateTime.Now,
                Total = total,
                HDL = hdl,
                LDL = ldl,
                Triglycerides = triglycerides
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            var measurement = Deserialize<CholesterolMeasurement>(json);

            return measurement;
        }

        /// <summary>
        /// Update an existing cholesterol measurement
        /// </summary>
        /// <param name="id"></param>
        /// <param name="personId"></param>
        /// <param name="date"></param>
        /// <param name="total"></param>
        /// <param name="hdl"></param>
        /// <param name="ldl"></param>
        /// <param name="triglycerides"></param>
        /// <returns></returns>
        public async Task<CholesterolMeasurement> UpdateAsync(
            int id, 
            int personId,
            DateTime? date,
            decimal total,
            decimal hdl,
            decimal ldl,
            decimal triglycerides)
        {
            dynamic template = new
            {
                Id = id,
                PersonId = personId,
                Date = date ?? DateTime.Now,
                Total = total,
                HDL = hdl,
                LDL = ldl,
                Triglycerides = triglycerides
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            var measurement = Deserialize<CholesterolMeasurement>(json);

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
        /// Request an export of cholesterol measurements to a named file in the API export folder
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
        /// Delete a cholesterol measurement from the database
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
        /// Return a list of cholesterol measurements
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<List<CholesterolMeasurement>> ListAsync(int personId, DateTime? from, DateTime? to)
        {
            // Determine the encoded date range
            (var encodedFromDate, var encodedToDate) = CalculateEncodedDateRange(from, to);

            // Request a list of cholesterol measurements
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            string route = $"{baseRoute}/{personId}/{encodedFromDate}/{encodedToDate}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no measurements in the database
            List<CholesterolMeasurement> measurements = Deserialize<List<CholesterolMeasurement>>(json);
            return measurements;
        }
    }
}

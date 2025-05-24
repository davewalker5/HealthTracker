using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Measurements;

namespace HealthTracker.Client.ApiClient
{
    public class BloodGlucoseMeasurementClient : HealthTrackerClientBase, IBloodGlucoseMeasurementClient
    {
        private const string RouteKey = "BloodGlucoseMeasurement";
        private const string ExportRouteKey = "ExportBloodGlucoseMeasurement";
        private const string ImportRouteKey = "ImportBloodGlucoseMeasurement";

        public BloodGlucoseMeasurementClient(IHealthTrackerHttpClient client, IHealthTrackerApplicationSettings settings, IAuthenticationTokenProvider tokenProvider)
            : base(client, settings, tokenProvider)
        {
        }

        /// <summary>
        /// Add a new blood glucose measurement to the database
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="date"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public async Task<BloodGlucoseMeasurement> AddAsync(int personId, DateTime? date, decimal level)
        {
            var measurementDate = date ?? DateTime.Now;
            dynamic template = new
            {
                PersonId = personId,
                Date = date,
                Level = level
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            var measurement = Deserialize<BloodGlucoseMeasurement>(json);

            return measurement;
        }

        /// <summary>
        /// Update an existing blood glucose measurement
        /// </summary>
        /// <param name="id"></param>
        /// <param name="personId"></param>
        /// <param name="date"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public async Task<BloodGlucoseMeasurement> UpdateAsync(int id, int personId, DateTime? date, decimal level)
        {
            var measurementDate = date ?? DateTime.Now;
            dynamic template = new
            {
                Id = id,
                PersonId = personId,
                Date = date,
                Level = level
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            var measurement = Deserialize<BloodGlucoseMeasurement>(json);

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
        /// Request an export of blood glucose measurements to a named file in the API export folder
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
        public async Task<BloodGlucoseMeasurement> GetAsync(int id)
        {
            // Request the measurement with the specified ID
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            string route = $"{baseRoute}/{id}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // Extract the measurement from the response
            var measurement = Deserialize<BloodGlucoseMeasurement>(json);
            return measurement;
        }

        /// <summary>
        /// Delete a blood glucose measurement from the database
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
        /// Return a list of blood glucose measurements
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<BloodGlucoseMeasurement>> ListAsync(int personId, DateTime? from, DateTime? to, int pageNumber, int pageSize)
        {
            // Determine the encoded date range
            (var encodedFromDate, var encodedToDate) = CalculateEncodedDateRange(from, to);

            // Request a list of blood glucose measurements
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            string route = $"{baseRoute}/{personId}/{encodedFromDate}/{encodedToDate}/{pageNumber}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no people in the database
            List<BloodGlucoseMeasurement> measurements = !string.IsNullOrEmpty(json) ? Deserialize<List<BloodGlucoseMeasurement>>(json) : null;
            return measurements;
        }
    }
}

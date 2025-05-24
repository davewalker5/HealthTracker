using System.Web;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Measurements;

namespace HealthTracker.Client.ApiClient
{
    public class WeightMeasurementClient : HealthTrackerClientBase, IWeightMeasurementClient
    {
        private const string RouteKey = "WeightMeasurement";
        private const string ExportRouteKey = "ExportWeightMeasurement";
        private const string ImportRouteKey = "ImportWeightMeasurement";

        public WeightMeasurementClient(IHealthTrackerHttpClient client, IHealthTrackerApplicationSettings settings, IAuthenticationTokenProvider tokenProvider)
            : base(client, settings, tokenProvider)
        {
        }

        /// <summary>
        /// Add a new weight measurement to the database
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="date"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public async Task<WeightMeasurement> AddAsync(int personId, DateTime? date, decimal weight)
        {
            var measurementDate = date ?? DateTime.Now;
            dynamic template = new
            {
                PersonId = personId,
                Date = new DateTime(measurementDate.Year, measurementDate.Month, measurementDate.Day, 0, 0, 0),
                Weight = weight
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            var measurement = Deserialize<WeightMeasurement>(json);

            return measurement;
        }

        /// <summary>
        /// Update an existing weight measurement
        /// </summary>
        /// <param name="id"></param>
        /// <param name="personId"></param>
        /// <param name="date"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public async Task<WeightMeasurement> UpdateAsync(int id, int personId, DateTime? date, decimal weight)
        {
            var measurementDate = date ?? DateTime.Now;
            dynamic template = new
            {
                Id = id,
                PersonId = personId,
                Date = new DateTime(measurementDate.Year, measurementDate.Month, measurementDate.Day, 0, 0, 0),
                Weight = weight
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            var measurement = Deserialize<WeightMeasurement>(json);

            return measurement;
        }

        /// <summary>
        /// Delete a weight measurement from the database
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
        /// Request an export of weight measurements to a named file in the API export folder
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
        public async Task<WeightMeasurement> GetAsync(int id)
        {
            // Request the measurement with the specified ID
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            string route = $"{baseRoute}/{id}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // Extract the measurement from the response
            var measurement = Deserialize<WeightMeasurement>(json);
            return measurement;
        }

        /// <summary>
        /// Return a list of weight measurements
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<WeightMeasurement>> ListAsync(int personId, DateTime? from, DateTime? to, int pageNumber, int pageSize)
        {
            // Determine the encoded date range
            (var encodedFromDate, var encodedToDate) = CalculateEncodedDateRange(from, to);

            // Request a list of weight measurements
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            string route = $"{baseRoute}/{personId}/{encodedFromDate}/{encodedToDate}/{pageNumber}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no measurements in the database
            List<WeightMeasurement> measurements = !string.IsNullOrEmpty(json) ? Deserialize<List<WeightMeasurement>>(json) : null;
            return measurements;
        }

        /// <summary>
        /// Calculate an average weight measurement for a person and date range
        /// </summary>
        /// <param name="id"></param>
        /// <param name="personId"></param>
        /// <param name="date"></param>
        /// <param name="systolic"></param>
        /// <param name="diastolic"></param>
        /// <returns></returns>
        public async Task<WeightMeasurement> CalculateAverageAsync(int personId, DateTime from, DateTime to)
        {
            var encodedFromDate = HttpUtility.UrlEncode(from.ToString(DateTimeFormat));
            var encodedToDate = HttpUtility.UrlEncode(to.ToString(DateTimeFormat));

            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            var route = $"{baseRoute}/average/{personId}/{encodedFromDate}/{encodedToDate}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // If there are no measurements in range, the result will be empty
            var measurement = !string.IsNullOrEmpty(json) ? Deserialize<WeightMeasurement>(json) : null;

            return measurement;
        }

        /// <summary>
        /// Calculate an average weight measurement for a person for the last "n" days
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public async Task<WeightMeasurement> CalculateAverageAsync(int personId, int days)
        {
            // Calculate an inclusive date range that ensures the whole of the start and end days are covered
            var now = DateTime.Now;
            var from = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).AddDays(-days + 1);
            var to = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);

            return await CalculateAverageAsync(personId, from, to);
        }
    }
}

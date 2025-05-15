using System.Web;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Measurements;

namespace HealthTracker.Client.ApiClient
{
    public class BloodOxygenSaturationMeasurementClient : HealthTrackerClientBase, IBloodOxygenSaturationMeasurementClient
    {
        private const string RouteKey = "BloodOxygenSaturationMeasurement";
        private const string ExportRouteKey = "ExportBloodOxygenSaturationMeasurement";
        private const string ImportRouteKey = "ImportBloodOxygenSaturationMeasurement";
        private const string ExportDailyAverageRouteKey = "ExportDailyAverageBloodOxygenSaturationRouteKey";

        public BloodOxygenSaturationMeasurementClient(IHealthTrackerHttpClient client, IHealthTrackerApplicationSettings settings, IAuthenticationTokenProvider tokenProvider)
            : base(client, settings, tokenProvider)
        {
        }

        /// <summary>
        /// Add a new % SPO2 measurement to the database
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="date"></param>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public async Task<BloodOxygenSaturationMeasurement> AddBloodOxygenSaturationMeasurementAsync(int personId, DateTime? date, decimal percentage)
        {
            var measurementDate = date ?? DateTime.Now;
            dynamic template = new
            {
                PersonId = personId,
                Date = new DateTime(measurementDate.Year, measurementDate.Month, measurementDate.Day, 0, 0, 0),
                Percentage = percentage
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            var measurement = Deserialize<BloodOxygenSaturationMeasurement>(json);

            return measurement;
        }

        /// <summary>
        /// Update an existing % SPO2 measurement
        /// </summary>
        /// <param name="id"></param>
        /// <param name="personId"></param>
        /// <param name="date"></param>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public async Task<BloodOxygenSaturationMeasurement> UpdateBloodOxygenSaturationMeasurementAsync(int id, int personId, DateTime? date, decimal percentage)
        {
            var measurementDate = date ?? DateTime.Now;
            dynamic template = new
            {
                Id = id,
                PersonId = personId,
                Date = new DateTime(measurementDate.Year, measurementDate.Month, measurementDate.Day, 0, 0, 0),
                Percentage = percentage
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            var measurement = Deserialize<BloodOxygenSaturationMeasurement>(json);

            return measurement;
        }

        /// <summary>
        /// Request an import of % SPO2 measurements from the content of a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task ImportBloodOxygenSaturationMeasurementsAsync(string filePath)
        {
            dynamic data = new{ Content = File.ReadAllText(filePath) };
            var json = Serialize(data);
            await SendIndirectAsync(ImportRouteKey, json, HttpMethod.Post);
        }

        /// <summary>
        /// Request an export of % SPO2 measurements to a named file in the API export folder
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task ExportBloodOxygenSaturationMeasurementsAsync(int personId, DateTime? from, DateTime? to, string fileName)
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
        public async Task<BloodOxygenSaturationMeasurement> Get(int id)
        {
            // Request the measurement with the specified ID
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            string route = $"{baseRoute}/{id}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // Extract the measurement from the response
            var measurement = Deserialize<BloodOxygenSaturationMeasurement>(json);
            return measurement;
        }

        /// <summary>
        /// Delete a % SPO2 measurement from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteBloodOxygenSaturationMeasurementAsync(int id)
        {
            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            var route = $"{baseRoute}/{id}";
            _ = await SendDirectAsync(route, null, HttpMethod.Delete);
        }

        /// <summary>
        /// Return a list of % SPO2 measurements
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<BloodOxygenSaturationMeasurement>> ListBloodOxygenSaturationMeasurementsAsync(int personId, DateTime? from, DateTime? to, int pageNumber, int pageSize)
        {
            // Determine the encoded date range
            (var encodedFromDate, var encodedToDate) = CalculateEncodedDateRange(from, to);

            // Request a list of % SPO2 measurements
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            string route = $"{baseRoute}/{personId}/{encodedFromDate}/{encodedToDate}/{pageNumber}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no people in the database
            List<BloodOxygenSaturationMeasurement> measurements = !string.IsNullOrEmpty(json) ? Deserialize<List<BloodOxygenSaturationMeasurement>>(json) : null;
            return measurements;
        }

        /// <summary>
        /// Calculate an average % SPO2 measurement for a person and date range
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<BloodOxygenSaturationMeasurement> CalculateAverageBloodOxygenSaturationAsync(int personId, DateTime from, DateTime to)
        {
            var encodedFromDate = HttpUtility.UrlEncode(from.ToString(DateTimeFormat));
            var encodedToDate = HttpUtility.UrlEncode(to.ToString(DateTimeFormat));

            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            var route = $"{baseRoute}/average/{personId}/{encodedFromDate}/{encodedToDate}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            var measurement = Deserialize<BloodOxygenSaturationMeasurement>(json);

            return measurement;
        }

        /// <summary>
        /// Calculate an average % SPO2 measurement for a person for the last "n" days
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public async Task<BloodOxygenSaturationMeasurement> CalculateAverageBloodOxygenSaturationAsync(int personId, int days)
        {
            // Calculate an inclusive date range that ensures the whole of the start and end days are covered
            var now = DateTime.Now;
            var from = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).AddDays(-days + 1);
            var to = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);

            return await CalculateAverageBloodOxygenSaturationAsync(personId, from, to);
        }

        /// <summary>
        /// Calculate a daily average % SPO2 reading for each date in a date range for a person
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<List<BloodOxygenSaturationMeasurement>> CalculateDailyAverageBloodOxygenSaturationAsync(int personId, DateTime from, DateTime to)
        {
            var encodedFromDate = HttpUtility.UrlEncode(from.ToString(DateTimeFormat));
            var encodedToDate = HttpUtility.UrlEncode(to.ToString(DateTimeFormat));

            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            var route = $"{baseRoute}/dailyaverage/{personId}/{encodedFromDate}/{encodedToDate}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            var measurement = Deserialize<List<BloodOxygenSaturationMeasurement>>(json);

            return measurement;
        }

        /// <summary>
        /// Export the daily average % SPO2 reading for each date in a date range for a person
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task ExportDailyAverageBloodOxygenSaturationAsync(int personId, DateTime from, DateTime to, string fileName)
        {
            dynamic data = new { PersonId = personId, From = from, To = to, FileName = fileName };
            var json = Serialize(data);
            await SendIndirectAsync(ExportDailyAverageRouteKey, json, HttpMethod.Post);
        }
    }
}

using System.Diagnostics.CodeAnalysis;
using System.Web;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Measurements;
using Microsoft.Extensions.Logging;

namespace HealthTracker.Client.ApiClient
{
    public class BloodPressureMeasurementClient : HealthTrackerClientBase, IBloodPressureMeasurementClient
    {
        private const string RouteKey = "BloodPressureMeasurement";
        private const string ExportRouteKey = "ExportBloodPressureMeasurement";
        private const string ImportRouteKey = "ImportBloodPressureMeasurement";
        private const string ImportOmronRouteKey = "ImportOmronBloodPressureMeasurement";
        private const string ExportDailyAverageRouteKey = "ExportDailyAverageBloodPressureRouteKey";

        public BloodPressureMeasurementClient(
            IHealthTrackerHttpClient client,
            IHealthTrackerApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ILogger<BloodPressureMeasurementClient> logger)
            : base(client, settings, tokenProvider, logger)
        {
        }

        /// <summary>
        /// Add a new blood pressure measurement to the database
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="date"></param>
        /// <param name="systolic"></param>
        /// <param name="diastolic"></param>
        /// <returns></returns>
        public async Task<BloodPressureMeasurement> AddAsync(int personId, DateTime? date, int systolic, int diastolic)
        {
            dynamic template = new
            {
                PersonId = personId,
                Date = date ?? DateTime.Now,
                Systolic = systolic,
                Diastolic = diastolic
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            var measurement = Deserialize<BloodPressureMeasurement>(json);

            return measurement;
        }

        /// <summary>
        /// Update an existing blood pressure measurement
        /// </summary>
        /// <param name="id"></param>
        /// <param name="personId"></param>
        /// <param name="date"></param>
        /// <param name="systolic"></param>
        /// <param name="diastolic"></param>
        /// <returns></returns>
        public async Task<BloodPressureMeasurement> UpdateAsync(int id, int personId, DateTime? date, int systolic, int diastolic)
        {
            dynamic template = new
            {
                Id = id,
                PersonId = personId,
                Date = date ?? DateTime.Now,
                Systolic = systolic,
                Diastolic = diastolic
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            var measurement = Deserialize<BloodPressureMeasurement>(json);

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
        /// Request an import of blood pressure measurements from the content of an OMRON-format XLSX file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        public async Task ImportOmronAsync(string filePath, int personId)
        {
            dynamic data = new
            {
                PersonId = personId,
                Content = Convert.ToBase64String(File.ReadAllBytes(filePath))
            };
            var json = Serialize(data);
            await SendIndirectAsync(ImportOmronRouteKey, json, HttpMethod.Post);
        }

        /// <summary>
        /// Request an export of blood pressure measurements to a named file in the API export folder
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
        public async Task<BloodPressureMeasurement> GetAsync(int id)
        {
            // Request the measurement with the specified ID
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            string route = $"{baseRoute}/{id}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // Extract the measurement from the response
            var measurement = Deserialize<BloodPressureMeasurement>(json);
            return measurement;
        }

        /// <summary>
        /// Delete a blood pressure measurement from the database
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
        /// Return a list of blood pressure measurements
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<BloodPressureMeasurement>> ListAsync(int personId, DateTime? from, DateTime? to, int pageNumber, int pageSize)
        {
            // Determine the encoded date range
            (var encodedFromDate, var encodedToDate) = CalculateEncodedDateRange(from, to);

            // Request a list of blood pressure measurements
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            string route = $"{baseRoute}/{personId}/{encodedFromDate}/{encodedToDate}/{pageNumber}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no measurements in the database
            List<BloodPressureMeasurement> measurements = Deserialize<List<BloodPressureMeasurement>>(json);
            return measurements;
        }

        /// <summary>
        /// Calculate an average blood pressure measurement for a person and date range
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<BloodPressureMeasurement> CalculateAverageAsync(int personId, DateTime from, DateTime to)
        {
            var encodedFromDate = HttpUtility.UrlEncode(from.ToString(DateTimeFormat));
            var encodedToDate = HttpUtility.UrlEncode(to.ToString(DateTimeFormat));

            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            var route = $"{baseRoute}/average/{personId}/{encodedFromDate}/{encodedToDate}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            var measurement = Deserialize<BloodPressureMeasurement>(json);

            return measurement;
        }

        /// <summary>
        /// Calculate an average blood pressure measurement for a person for the last "n" days
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        public async Task<BloodPressureMeasurement> CalculateAverageAsync(int personId, int days)
        {
            // Calculate an inclusive date range that ensures the whole of the start and end days are covered
            var now = DateTime.Now;
            var from = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).AddDays(-days + 1);
            var to = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);

            return await CalculateAverageAsync(personId, from, to);
        }

        /// <summary>
        /// Calculate a daily average blood pressure reading for each date in a date range for a person
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<List<BloodPressureMeasurement>> CalculateDailyAverageAsync(int personId, DateTime from, DateTime to)
        {
            var encodedFromDate = HttpUtility.UrlEncode(from.ToString(DateTimeFormat));
            var encodedToDate = HttpUtility.UrlEncode(to.ToString(DateTimeFormat));

            var baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            var route = $"{baseRoute}/dailyaverage/{personId}/{encodedFromDate}/{encodedToDate}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            var measurement = Deserialize<List<BloodPressureMeasurement>>(json);

            return measurement;
        }

        /// <summary>
        /// Export the daily average blood pressure reading for each date in a date range for a person
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task ExportDailyAverageAsync(int personId, DateTime from, DateTime to, string fileName)
        {
            dynamic data = new { PersonId = personId, From = from, To = to, FileName = fileName };
            var json = Serialize(data);
            Console.WriteLine(json);
            await SendIndirectAsync(ExportDailyAverageRouteKey, json, HttpMethod.Post);
        }
    }
}

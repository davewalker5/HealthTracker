using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Measurements;
using Microsoft.Extensions.Logging;

namespace HealthTracker.Client.ApiClient
{
    public class ExerciseMeasurementClient : HealthTrackerClientBase, IExerciseMeasurementClient
    {
        private const string RouteKey = "ExerciseMeasurement";
        private const string ExportRouteKey = "ExportExerciseMeasurement";
        private const string ImportRouteKey = "ImportExerciseMeasurement";

        public ExerciseMeasurementClient(
            IHealthTrackerHttpClient client,
            IHealthTrackerApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ILogger<ExerciseMeasurementClient> logger)
            : base(client, settings, tokenProvider, logger)
        {
        }

        /// <summary>
        /// Add a new exercise measurement to the database
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="activityTypeId"></param>
        /// <param name="date"></param>
        /// <param name="duration"></param>
        /// <param name="distance"></param>
        /// <param name="calories"></param>
        /// <param name="minimumHeartRate"></param>
        /// <param name="maximumHeartRate"></param>
        /// <returns></returns>
        public async Task<ExerciseMeasurement> AddAsync(
            int personId,
            int activityTypeId,
            DateTime? date,
            int duration,
            decimal? distance,
            int calories,
            int minimumHeartRate,
            int maximumHeartRate)
        {
            var measurementDistance = distance ?? 0;
            if (measurementDistance < 0) measurementDistance = 0;

            dynamic template = new
            {
                PersonId = personId,
                ActivityTypeId = activityTypeId,
                Date = date ?? DateTime.Now,
                Duration = duration,
                Distance = measurementDistance,
                Calories = calories,
                MinimumHeartRate = minimumHeartRate,
                MaximumHeartRate = maximumHeartRate
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            var measurement = Deserialize<ExerciseMeasurement>(json);

            return measurement;
        }

        /// <summary>
        /// Update an existing exercise measurement
        /// </summary>
        /// <param name="id"></param>
        /// <param name="personId"></param>
        /// <param name="activityTypeId"></param>
        /// <param name="date"></param>
        /// <param name="duration"></param>
        /// <param name="distance"></param>
        /// <param name="calories"></param>
        /// <param name="minimumHeartRate"></param>
        /// <param name="maximumHeartRate"></param>
        /// <returns></returns>
        public async Task<ExerciseMeasurement> UpdateAsync(
            int id,
            int personId,
            int activityTypeId,
            DateTime? date,
            int duration,
            decimal? distance,
            int calories,
            int minimumHeartRate,
            int maximumHeartRate)
        {
            var measurementDistance = distance ?? 0;
            if (measurementDistance < 0) measurementDistance = 0;

            dynamic template = new
            {
                Id = id,
                PersonId = personId,
                ActivityTypeId = activityTypeId,
                Date = date ?? DateTime.Now,
                Duration = duration,
                Distance = measurementDistance,
                Calories = calories,
                MinimumHeartRate = minimumHeartRate,
                MaximumHeartRate = maximumHeartRate
            };

            var data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            var measurement = Deserialize<ExerciseMeasurement>(json);

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
        /// Request an export of exercise measurements to a named file in the API export folder
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
        public async Task<ExerciseMeasurement> GetAsync(int id)
        {
            // Request the measurement with the specified ID
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            string route = $"{baseRoute}/{id}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // Extract the measurement from the response
            var measurement = Deserialize<ExerciseMeasurement>(json);
            return measurement;
        }

        /// <summary>
        /// Delete a exercise measurement from the database
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
        /// Return a list of exercise measurements
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<ExerciseMeasurement>> ListAsync(int personId, DateTime? from, DateTime? to, int pageNumber, int pageSize)
        {
            // Determine the encoded date range
            (var encodedFromDate, var encodedToDate) = CalculateEncodedDateRange(from, to);

            // Request a list of exercise measurements
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            string route = $"{baseRoute}/{personId}/{encodedFromDate}/{encodedToDate}/{pageNumber}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // The returned JSON will be empty if there are no measurements in the database
            List<ExerciseMeasurement> measurements = Deserialize<List<ExerciseMeasurement>>(json);
            return measurements;
        }

        /// <summary>
        /// Request a summary of exercise measurements for a person, optional activity type and date range
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="activityTypeId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ExerciseSummary>> SummariseAsync(int personId, int? activityTypeId, DateTime from, DateTime to)
        {
            // Determine the encoded date range
            (var encodedFromDate, var encodedToDate) = CalculateEncodedDateRange(from, to);

            // Request the summary
            string baseRoute = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}";
            string route = $"{baseRoute}/summarise/{personId}/{activityTypeId ?? 0}/{encodedFromDate}/{encodedToDate}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);

            // Deserialise the JSON response into a collection of summary objects
            IEnumerable<ExerciseSummary> summaries = Deserialize<IEnumerable<ExerciseSummary>>(json);
            return summaries;
        }
    }
}

using System.Text.Json;
using System.Web;
using HealthTracker.Client.ApiClient;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.Logic.Extensions;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.Exercise
{
    [TestClass]
    public class ExerciseMeasurementClientTest
    {
        private readonly string ApiToken = DataGenerator.RandomApiToken();
        private MockHealthTrackerHttpClient _httpClient = new();
        private IExerciseMeasurementClient _client;
        private string _filePath;

        private readonly HealthTrackerApplicationSettings _settings = new()
        {
            DefaultTimePeriodDays = 7,
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "ExerciseMeasurement", Route = "/exercisemeasurement" },
                new() { Name = "ExportExerciseMeasurement", Route = "/export/exercise" },
                new() { Name = "ImportExerciseMeasurement", Route = "/import/exercise" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(ApiToken);
            _client = new ExerciseMeasurementClient(_httpClient, _settings, provider.Object);
        }

        [TestCleanup]
        public void CleanUp()
        {
            if (!string.IsNullOrEmpty(_filePath) && File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }

        [TestMethod]
        public async Task AddTest()
        {
            var measurement = DataGenerator.RandomExerciseMeasurement(2024);
            var json = JsonSerializer.Serialize(measurement);
            _httpClient.AddResponse(json);

            var added = await _client.AddAsync(
                measurement.PersonId,
                measurement.ActivityTypeId,
                measurement.Date,
                measurement.Duration,
                measurement.Distance,
                measurement.Calories,
                measurement.MinimumHeartRate,
                measurement.MaximumHeartRate);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.IsNotNull(added);
            Assert.AreEqual(measurement.Id, added.Id);
            Assert.AreEqual(measurement.PersonId, added.PersonId);
            Assert.AreEqual(measurement.Date, added.Date);
            Assert.AreEqual(measurement.ActivityTypeId, added.ActivityTypeId);
            Assert.AreEqual(measurement.Duration, added.Duration);
            Assert.AreEqual(measurement.Distance, added.Distance);
            Assert.AreEqual(measurement.Calories, added.Calories);
            Assert.AreEqual(measurement.MinimumHeartRate, added.MinimumHeartRate);
            Assert.AreEqual(measurement.MaximumHeartRate, added.MaximumHeartRate);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var measurement = DataGenerator.RandomExerciseMeasurement(2024);
            var json = JsonSerializer.Serialize(measurement);
            _httpClient.AddResponse(json);

            var updated = await _client.UpdateAsync(
                measurement.Id,
                measurement.PersonId,
                measurement.ActivityTypeId,
                measurement.Date,
                measurement.Duration,
                measurement.Distance,
                measurement.Calories,
                measurement.MinimumHeartRate,
                measurement.MaximumHeartRate);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.IsNotNull(updated);
            Assert.AreEqual(measurement.Id, updated.Id);
            Assert.AreEqual(measurement.PersonId, updated.PersonId);
            Assert.AreEqual(measurement.Date, updated.Date);
            Assert.AreEqual(measurement.ActivityTypeId, updated.ActivityTypeId);
            Assert.AreEqual(measurement.Duration, updated.Duration);
            Assert.AreEqual(measurement.Distance, updated.Distance);
            Assert.AreEqual(measurement.Calories, updated.Calories);
            Assert.AreEqual(measurement.MinimumHeartRate, updated.MinimumHeartRate);
            Assert.AreEqual(measurement.MaximumHeartRate, updated.MaximumHeartRate);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            var id = DataGenerator.RandomId();
            await _client.DeleteAsync(id);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Delete, _httpClient.Requests[0].Method);
            Assert.AreEqual($"{_settings.ApiRoutes[0].Route}/{id}", _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
        }

        [TestMethod]
        public async Task GetTest()
        {
            var measurement = DataGenerator.RandomExerciseMeasurement(2024);
            var json = JsonSerializer.Serialize(measurement);
            _httpClient.AddResponse(json);

            var retrieved = await _client.GetAsync(measurement.Id);
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{measurement.Id}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(measurement.Id, retrieved.Id);
            Assert.AreEqual(measurement.PersonId, retrieved.PersonId);
            Assert.AreEqual(measurement.Date, retrieved.Date);
            Assert.AreEqual(measurement.Duration, retrieved.Duration);
            Assert.AreEqual(measurement.Distance, retrieved.Distance);
            Assert.AreEqual(measurement.Calories, retrieved.Calories);
            Assert.AreEqual(measurement.MinimumHeartRate, retrieved.MinimumHeartRate);
            Assert.AreEqual(measurement.MaximumHeartRate, retrieved.MaximumHeartRate);
        }

        [TestMethod]
        public async Task ListWithNoDateRangeTest()
        {
            var measurement = DataGenerator.RandomExerciseMeasurement(2024);
            var json = JsonSerializer.Serialize(new List<dynamic> { measurement });
            _httpClient.AddResponse(json);

            var measurements = await _client.ListAsync(measurement.PersonId, null, null, 1, int.MaxValue);

            var expectedTo = DateTime.Now;
            var expectedFrom = expectedTo.AddDays(-_settings.DefaultTimePeriodDays);
            var encodedFrom = HttpUtility.UrlEncode(expectedFrom.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(expectedTo.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{measurement.PersonId}/{encodedFrom}/{encodedTo}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(measurements);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(measurement.Id, measurements[0].Id);
            Assert.AreEqual(measurement.PersonId, measurements[0].PersonId);
            Assert.AreEqual(measurement.Date, measurements[0].Date);
            Assert.AreEqual(measurement.ActivityTypeId, measurements[0].ActivityTypeId);
            Assert.AreEqual(measurement.Duration, measurements[0].Duration);
            Assert.AreEqual(measurement.Distance, measurements[0].Distance);
            Assert.AreEqual(measurement.Calories, measurements[0].Calories);
            Assert.AreEqual(measurement.MinimumHeartRate, measurements[0].MinimumHeartRate);
            Assert.AreEqual(measurement.MaximumHeartRate, measurements[0].MaximumHeartRate);
        }

        [TestMethod]
        public async Task ListWithFromDateOnlyTest()
        {
            var measurement = DataGenerator.RandomExerciseMeasurement(2024);
            var json = JsonSerializer.Serialize(new List<dynamic> { measurement });
            _httpClient.AddResponse(json);

            var from = measurement.Date.AddDays(-DataGenerator.RandomInt(30,90));
            var measurements = await _client.ListAsync(measurement.PersonId, from, null, 1, int.MaxValue);

            var expectedTo = DateTime.Now;
            var encodedFrom = HttpUtility.UrlEncode(from.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(expectedTo.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{measurement.PersonId}/{encodedFrom}/{encodedTo}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(measurements);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(measurement.Id, measurements[0].Id);
            Assert.AreEqual(measurement.PersonId, measurements[0].PersonId);
            Assert.AreEqual(measurement.Date, measurements[0].Date);
            Assert.AreEqual(measurement.ActivityTypeId, measurements[0].ActivityTypeId);
            Assert.AreEqual(measurement.Duration, measurements[0].Duration);
            Assert.AreEqual(measurement.Distance, measurements[0].Distance);
            Assert.AreEqual(measurement.Calories, measurements[0].Calories);
            Assert.AreEqual(measurement.MinimumHeartRate, measurements[0].MinimumHeartRate);
            Assert.AreEqual(measurement.MaximumHeartRate, measurements[0].MaximumHeartRate);
        }

        [TestMethod]
        public async Task ListWithEndDateOnlyTest()
        {
            var measurement = DataGenerator.RandomExerciseMeasurement(2024);
            var json = JsonSerializer.Serialize(new List<dynamic> { measurement });
            _httpClient.AddResponse(json);

            var to = measurement.Date.AddDays(-DataGenerator.RandomInt(30,90));
            var measurements = await _client.ListAsync(measurement.PersonId, null, to, 1, int.MaxValue);

            var expectedFrom = to.AddDays(-_settings.DefaultTimePeriodDays);
            var encodedFrom = HttpUtility.UrlEncode(expectedFrom.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(to.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{measurement.PersonId}/{encodedFrom}/{encodedTo}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(measurements);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(measurement.Id, measurements[0].Id);
            Assert.AreEqual(measurement.PersonId, measurements[0].PersonId);
            Assert.AreEqual(measurement.Date, measurements[0].Date);
            Assert.AreEqual(measurement.ActivityTypeId, measurements[0].ActivityTypeId);
            Assert.AreEqual(measurement.Duration, measurements[0].Duration);
            Assert.AreEqual(measurement.Distance, measurements[0].Distance);
            Assert.AreEqual(measurement.Calories, measurements[0].Calories);
            Assert.AreEqual(measurement.MinimumHeartRate, measurements[0].MinimumHeartRate);
            Assert.AreEqual(measurement.MaximumHeartRate, measurements[0].MaximumHeartRate);
        }

        [TestMethod]
        public async Task ListWithFromAndToDatesTest()
        {
            var measurement = DataGenerator.RandomExerciseMeasurement(2024);
            var json = JsonSerializer.Serialize(new List<dynamic> { measurement });
            _httpClient.AddResponse(json);

            var from = measurement.Date.AddDays(-DataGenerator.RandomInt(30,90));
            var to = measurement.Date.AddDays(DataGenerator.RandomInt(30,90));
            var measurements = await _client.ListAsync(measurement.PersonId, from, to, 1, int.MaxValue);

            var encodedFrom = HttpUtility.UrlEncode(from.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(to.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{measurement.PersonId}/{encodedFrom}/{encodedTo}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(measurements);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(measurement.Id, measurements[0].Id);
            Assert.AreEqual(measurement.PersonId, measurements[0].PersonId);
            Assert.AreEqual(measurement.Date, measurements[0].Date);
            Assert.AreEqual(measurement.ActivityTypeId, measurements[0].ActivityTypeId);
            Assert.AreEqual(measurement.Duration, measurements[0].Duration);
            Assert.AreEqual(measurement.Distance, measurements[0].Distance);
            Assert.AreEqual(measurement.Calories, measurements[0].Calories);
            Assert.AreEqual(measurement.MinimumHeartRate, measurements[0].MinimumHeartRate);
            Assert.AreEqual(measurement.MaximumHeartRate, measurements[0].MaximumHeartRate);
        }

        [TestMethod]
        public async Task SummariseTest()
        {
            var personId = DataGenerator.RandomId();
            var summary = DataGenerator.RandomExerciseSummary();
            var json = JsonSerializer.Serialize(new List<dynamic> { summary });
            _httpClient.AddResponse(json);

            var summaries = await _client.SummariseAsync(personId, null, summary.From.Value, summary.To.Value);

            var encodedFrom = HttpUtility.UrlEncode(summary.From.Value.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(summary.To.Value.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/summarise/{personId}/0/{encodedFrom}/{encodedTo}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.AreEqual(1, summaries.Count());
            Assert.AreEqual(summary.PersonId, summaries.First().PersonId);
            Assert.AreEqual(summary.ActivityTypeId, summaries.First().ActivityTypeId);
            Assert.AreEqual(summary.From, summaries.First().From);
            Assert.AreEqual(summary.Count, summaries.First().Count);
            Assert.AreEqual(summary.TotalDuration, summaries.First().TotalDuration);
            Assert.AreEqual(summary.TotalDistance, summaries.First().TotalDistance);
            Assert.AreEqual(summary.TotalCalories, summaries.First().TotalCalories);
            Assert.AreEqual(summary.MinimumHeartRate, summaries.First().MinimumHeartRate);
            Assert.AreEqual(summary.MaximumHeartRate, summaries.First().MaximumHeartRate);
        }

        [TestMethod]
        public async Task ImportTest()
        {
            _httpClient.AddResponse("");

            var person = DataGenerator.RandomPerson(16, 90);
            var activityType = DataGenerator.RandomActivityType();
            var measurement = DataGenerator.RandomExerciseMeasurement(person.Id, activityType.Id, DataGenerator.RandomDateInYear(2024));
            var record = $"""{measurement.PersonId}"",""{person.Name}"",""{measurement.Date:dd-MMM-yyyy HH:mm:ss}"",""{activityType.Description}"",""{measurement.Duration.ToFormattedDuration()}"",""{measurement.Distance}"",""{measurement.Calories}"",""{measurement.MinimumHeartRate}"",""{measurement.MaximumHeartRate}""";

            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _client.ImportFromFileAsync(_filePath);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "ImportExerciseMeasurement").Route, _httpClient.Requests[0].Uri);
        }

        [TestMethod]
        public async Task ExportTest()
        {
            _httpClient.AddResponse("");
            _filePath = DataGenerator.TemporaryCsvFilePath();

            await _client.ExportAsync(DataGenerator.RandomId(), null, null, _filePath);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "ExportExerciseMeasurement").Route, _httpClient.Requests[0].Uri);
        }
    }
}
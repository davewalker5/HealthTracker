using System.Text.Json;
using System.Web;
using HealthTracker.Client.ApiClient;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.Tests.Mocks;
using Microsoft.Extensions.Logging;
using Moq;

namespace HealthTracker.Tests.Weight
{
    [TestClass]
    public class WeightMeasurementClientTest
    {
        private const string ApiToken = "This is a token";
        private MockHealthTrackerHttpClient _httpClient = new();
        private IWeightMeasurementClient _client;
        private string _filePath;

        private readonly HealthTrackerApplicationSettings _settings = new()
        {
            DefaultTimePeriodDays = 7,
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "WeightMeasurement", Route = "/weightmeasurement" },
                new() { Name = "ImportWeightMeasurement", Route = "/import/weightmeasurement" },
                new() { Name = "ExportWeightMeasurement", Route = "/export/weightmeasurement" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(ApiToken);
            var logger = new Mock<ILogger<WeightMeasurementClient>>();
            _client = new WeightMeasurementClient(_httpClient, _settings, provider.Object, logger.Object);
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
            var measurement = DataGenerator.RandomWeightMeasurement(2024, 50, 100);
            var json = JsonSerializer.Serialize(measurement);
            _httpClient.AddResponse(json);

            var added = await _client.AddAsync(measurement.PersonId, measurement.Date, measurement.Weight);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "WeightMeasurement").Route, _httpClient.Requests[0].Uri);

            Assert.IsNotNull(added);
            Assert.AreEqual(measurement.Id, added.Id);
            Assert.AreEqual(measurement.PersonId, added.PersonId);
            Assert.AreEqual(measurement.Date, added.Date);
            Assert.AreEqual(measurement.Weight, added.Weight);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var measurement = DataGenerator.RandomWeightMeasurement(2024, 50, 100);
            var json = JsonSerializer.Serialize(measurement);
            _httpClient.AddResponse(json);

            var updated = await _client.UpdateAsync(measurement.Id, measurement.PersonId, measurement.Date, measurement.Weight);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "WeightMeasurement").Route, _httpClient.Requests[0].Uri);

            Assert.IsNotNull(updated);
            Assert.AreEqual(measurement.Id, updated.Id);
            Assert.AreEqual(measurement.PersonId, updated.PersonId);
            Assert.AreEqual(measurement.Date, updated.Date);
            Assert.AreEqual(measurement.Weight, updated.Weight);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            var id = DataGenerator.RandomId();
            await _client.DeleteAsync(id);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Delete, _httpClient.Requests[0].Method);
            Assert.AreEqual($"{_settings.ApiRoutes.First(x => x.Name == "WeightMeasurement").Route}/{id}", _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
        }

        [TestMethod]
        public async Task GetTest()
        {
            var measurement = DataGenerator.RandomWeightMeasurement(70, 90);
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
            Assert.AreEqual(measurement.Weight, retrieved.Weight);
        }

        [TestMethod]
        public async Task ListWithNoDateRangeTest()
        {
            var measurement = DataGenerator.RandomWeightMeasurement(50, 100);
            var json = JsonSerializer.Serialize(new List<dynamic> { measurement });
            _httpClient.AddResponse(json);

            var measurements = await _client.ListAsync(measurement.PersonId, null, null, 1, int.MaxValue);

            var expectedTo = DateTime.Now;
            var expectedFrom = expectedTo.AddDays(-_settings.DefaultTimePeriodDays);
            var encodedFrom = HttpUtility.UrlEncode(expectedFrom.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(expectedTo.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes.First(x => x.Name == "WeightMeasurement").Route}/{measurement.PersonId}/{encodedFrom}/{encodedTo}/1/{int.MaxValue}";

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
            Assert.AreEqual(measurement.Weight, measurements[0].Weight);
        }

        [TestMethod]
        public async Task ListWithFromDateOnlyTest()
        {
            var measurement = DataGenerator.RandomWeightMeasurement(2024, 50, 100);
            var json = JsonSerializer.Serialize(new List<dynamic> { measurement });
            _httpClient.AddResponse(json);

            var from = measurement.Date.AddDays(-DataGenerator.RandomInt(30, 90));
            var measurements = await _client.ListAsync(measurement.PersonId, from, null, 1, int.MaxValue);

            var expectedTo = DateTime.Now;
            var encodedFrom = HttpUtility.UrlEncode(from.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(expectedTo.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes.First(x => x.Name == "WeightMeasurement").Route}/{measurement.PersonId}/{encodedFrom}/{encodedTo}/1/{int.MaxValue}";

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
            Assert.AreEqual(measurement.Weight, measurements[0].Weight);
        }

        [TestMethod]
        public async Task ListWithEndDateOnlyTest()
        {
            var measurement = DataGenerator.RandomWeightMeasurement(50, 100);
            var json = JsonSerializer.Serialize(new List<dynamic> { measurement });
            _httpClient.AddResponse(json);

            var to = measurement.Date.AddDays(DataGenerator.RandomInt(30, 90));
            var measurements = await _client.ListAsync(measurement.PersonId, null, to, 1, int.MaxValue);

            var expectedFrom = to.AddDays(-_settings.DefaultTimePeriodDays);
            var encodedFrom = HttpUtility.UrlEncode(expectedFrom.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(to.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes.First(x => x.Name == "WeightMeasurement").Route}/{measurement.PersonId}/{encodedFrom}/{encodedTo}/1/{int.MaxValue}";

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
            Assert.AreEqual(measurement.Weight, measurements[0].Weight);
        }

        [TestMethod]
        public async Task ListWithFromAndToDatesTest()
        {
            var measurement = DataGenerator.RandomWeightMeasurement(2024, 50, 100);
            var json = JsonSerializer.Serialize(new List<dynamic> { measurement });
            _httpClient.AddResponse(json);

            var from = measurement.Date.AddDays(DataGenerator.RandomInt(30, 90));
            var to = measurement.Date.AddDays(DataGenerator.RandomInt(30, 90));
            var measurements = await _client.ListAsync(measurement.PersonId, from, to, 1, int.MaxValue);

            var encodedFrom = HttpUtility.UrlEncode(from.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(to.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes.First(x => x.Name == "WeightMeasurement").Route}/{measurement.PersonId}/{encodedFrom}/{encodedTo}/1/{int.MaxValue}";

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
            Assert.AreEqual(measurement.Weight, measurements[0].Weight);
        }

        [TestMethod]
        public async Task ImportTest()
        {
            _httpClient.AddResponse("");

            var person = DataGenerator.RandomPerson(16, 90);
            var measurement = DataGenerator.RandomWeightMeasurement(person.Id, 2024, 50, 100);
            var record = $@"""{person.Id}"",""{person.Name}"",""{measurement.Date:dd-MMM-yyyy HH:mm:ss}"",""{measurement.Weight}";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _client.ImportFromFileAsync(_filePath);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "ImportWeightMeasurement").Route, _httpClient.Requests[0].Uri);
        }

        [TestMethod]
        public async Task ExportTest()
        {
            _httpClient.AddResponse("");
            var personId = DataGenerator.RandomId();
            _filePath = DataGenerator.TemporaryCsvFilePath();

            await _client.ExportAsync(personId, null, null, _filePath);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "ExportWeightMeasurement").Route, _httpClient.Requests[0].Uri);
        }

        [TestMethod]
        public async Task CalculateAverageForDateRangeTest()
        {
            var measurement = DataGenerator.RandomWeightMeasurement(2024, 50, 100);
            var json = JsonSerializer.Serialize(measurement);
            _httpClient.AddResponse(json);

            var from = measurement.Date.AddDays(-DataGenerator.RandomInt(30,90));
            var to = measurement.Date.AddDays(DataGenerator.RandomInt(30,90));
            var average = await _client.CalculateAverageAsync(measurement.PersonId, from, to);

            var encodedFrom = HttpUtility.UrlEncode(from.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(to.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes.First(x => x.Name == "WeightMeasurement").Route}/average/{measurement.PersonId}/{encodedFrom}/{encodedTo}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(average);
            Assert.AreEqual(measurement.Id, average.Id);
            Assert.AreEqual(measurement.PersonId, average.PersonId);
            Assert.AreEqual(measurement.Date, average.Date);
            Assert.AreEqual(measurement.Weight, average.Weight);
        }

        [TestMethod]
        public async Task CalculateAverageForNumberOfDaysTest()
        {
            var measurement = DataGenerator.RandomWeightMeasurement(50, 100);
            var json = JsonSerializer.Serialize(measurement);
            _httpClient.AddResponse(json);

            var average = await _client.CalculateAverageAsync(measurement.PersonId, _settings.DefaultTimePeriodDays);

            var expectedRoute = $"{_settings.ApiRoutes.First(x => x.Name == "WeightMeasurement").Route}/average/{measurement.PersonId}/";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.IsTrue(_httpClient.Requests[0].Uri.StartsWith(expectedRoute));

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(average);
            Assert.AreEqual(measurement.Id, average.Id);
            Assert.AreEqual(measurement.PersonId, average.PersonId);
            Assert.AreEqual(measurement.Date, average.Date);
            Assert.AreEqual(measurement.Weight, average.Weight);
        }
    }
}
using System.Text.Json;
using System.Web;
using HealthTracker.Client.ApiClient;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.Tests.Mocks;
using Microsoft.Extensions.Logging;
using Moq;

namespace HealthTracker.Tests.BloodOxygenSaturation
{
    [TestClass]
    public class BloodOxygenSaturationMeasurementClientTest
    {
        private readonly string _apiToken = DataGenerator.RandomApiToken();
        private readonly MockHealthTrackerHttpClient _httpClient = new();
        private IBloodOxygenSaturationMeasurementClient _client;
        private string _filePath;

        private readonly HealthTrackerApplicationSettings _settings = new()
        {
            DefaultTimePeriodDays = 7,
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "BloodOxygenSaturationMeasurement", Route = "/bloodoxygensaturationmeasurement" },
                new() { Name = "ExportBloodOxygenSaturationMeasurement", Route = "/export/bloodoxygensaturationmeasurement" },
                new() { Name = "ImportBloodOxygenSaturationMeasurement", Route = "/import/bloodoxygensaturationmeasurement" },
                new() { Name = "ExportDailyAverageBloodOxygenSaturationRouteKey", Route = "/export/dailyaveragebloodoxygensaturation" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(_apiToken);
            var logger = new Mock<ILogger<BloodOxygenSaturationMeasurementClient>>();
            _client = new BloodOxygenSaturationMeasurementClient(_httpClient, _settings, provider.Object, logger.Object);
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
            var personId = DataGenerator.RandomId();
            var date = DataGenerator.RandomDateInYear(2024);
            var percentage = DataGenerator.RandomSPO2Value();
            var json = JsonSerializer.Serialize(new { PersonId = personId, Date = date, Percentage = percentage });
            _httpClient.AddResponse(json);

            var measurement = await _client.AddAsync(personId, date, percentage);

            Assert.AreEqual($"Bearer {_apiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.AreEqual(json, await _httpClient.Requests[0].Content.ReadAsStringAsync());
            Assert.IsNotNull(measurement);
            Assert.AreEqual(personId, measurement.PersonId);
            Assert.AreEqual(date, measurement.Date);
            Assert.AreEqual(percentage, measurement.Percentage);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var id = DataGenerator.RandomId();
            var personId = DataGenerator.RandomId();
            var date = DataGenerator.RandomDateInYear(2024);
            var percentage = DataGenerator.RandomSPO2Value();
            var json = JsonSerializer.Serialize(new { Id = id, PersonId = personId, Date = date, Percentage = percentage });
            _httpClient.AddResponse(json);

            var measurement = await _client.UpdateAsync(id, personId, date, percentage);

            Assert.AreEqual($"Bearer {_apiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.AreEqual(json, await _httpClient.Requests[0].Content.ReadAsStringAsync());
            Assert.IsNotNull(measurement);
            Assert.AreEqual(id, measurement.Id);
            Assert.AreEqual(personId, measurement.PersonId);
            Assert.AreEqual(date, measurement.Date);
            Assert.AreEqual(percentage, measurement.Percentage);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            var id = DataGenerator.RandomId();
            await _client.DeleteAsync(id);

            Assert.AreEqual($"Bearer {_apiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Delete, _httpClient.Requests[0].Method);
            Assert.AreEqual($"{_settings.ApiRoutes[0].Route}/{id}", _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
        }

        [TestMethod]
        public async Task GetTest()
        {
            var personId = DataGenerator.RandomId();
            var measurement = DataGenerator.RandomSPO2Measurement(personId, 2024);
            var json = JsonSerializer.Serialize(measurement);
            _httpClient.AddResponse(json);

            var retrieved = await _client.GetAsync(measurement.Id);
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{measurement.Id}";

            Assert.AreEqual($"Bearer {_apiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(measurement.Id, retrieved.Id);
            Assert.AreEqual(personId, retrieved.PersonId);
            Assert.AreEqual(measurement.Date, retrieved.Date);
            Assert.AreEqual(measurement.Percentage, retrieved.Percentage);
        }

        [TestMethod]
        public async Task ListWithNoDateRangeTest()
        {
            var id = DataGenerator.RandomId();
            var personId = DataGenerator.RandomId();
            var date = DataGenerator.RandomDateInYear(2024);
            var percentage = DataGenerator.RandomSPO2Value();
            var json = JsonSerializer.Serialize(new List<dynamic>()
            {
                new
                {
                    Id = id,
                    PersonId = personId,
                    Date = date,
                    Percentage = percentage
                }
            });
            _httpClient.AddResponse(json);

            var measurements = await _client.ListAsync(personId, null, null, 1, int.MaxValue);

            var expectedTo = DateTime.Now;
            var expectedFrom = expectedTo.AddDays(-_settings.DefaultTimePeriodDays);
            var encodedFrom = HttpUtility.UrlEncode(expectedFrom.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(expectedTo.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{personId}/{encodedFrom}/{encodedTo}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {_apiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(measurements);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(id, measurements[0].Id);
            Assert.AreEqual(personId, measurements[0].PersonId);
            Assert.AreEqual(date, measurements[0].Date);
            Assert.AreEqual(percentage, measurements[0].Percentage);
        }

        [TestMethod]
        public async Task ListWithFromDateOnlyTest()
        {
            var id = DataGenerator.RandomId();
            var personId = DataGenerator.RandomId();
            var date = DataGenerator.RandomDateInYear(2024);
            var percentage = DataGenerator.RandomSPO2Value();
            var json = JsonSerializer.Serialize(new List<dynamic>()
            {
                new
                {
                    Id = id,
                    PersonId = personId,
                    Date = date,
                    Percentage = percentage
                }
            });
            _httpClient.AddResponse(json);

            var from = date.AddDays(-DataGenerator.RandomInt(30, 90));
            var measurements = await _client.ListAsync(personId, from, null, 1, int.MaxValue);

            var expectedTo = DateTime.Now;
            var encodedFrom = HttpUtility.UrlEncode(from.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(expectedTo.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{personId}/{encodedFrom}/{encodedTo}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {_apiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(measurements);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(id, measurements[0].Id);
            Assert.AreEqual(personId, measurements[0].PersonId);
            Assert.AreEqual(date, measurements[0].Date);
            Assert.AreEqual(percentage, measurements[0].Percentage);
        }

        [TestMethod]
        public async Task ListWithEndDateOnlyTest()
        {
            var id = DataGenerator.RandomId();
            var personId = DataGenerator.RandomId();
            var date = DataGenerator.RandomDateInYear(2024);
            var percentage = DataGenerator.RandomSPO2Value();
            var json = JsonSerializer.Serialize(new List<dynamic>()
            {
                new
                {
                    Id = id,
                    PersonId = personId,
                    Date = date,
                    Percentage = percentage
                }
            });
            _httpClient.AddResponse(json);

            var to = date.AddDays(DataGenerator.RandomInt(30, 90));
            var measurements = await _client.ListAsync(personId, null, to, 1, int.MaxValue);

            var expectedFrom = to.AddDays(-_settings.DefaultTimePeriodDays);
            var encodedFrom = HttpUtility.UrlEncode(expectedFrom.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(to.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{personId}/{encodedFrom}/{encodedTo}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {_apiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(measurements);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(id, measurements[0].Id);
            Assert.AreEqual(personId, measurements[0].PersonId);
            Assert.AreEqual(date, measurements[0].Date);
            Assert.AreEqual(percentage, measurements[0].Percentage);
        }

        [TestMethod]
        public async Task ListWithFromAndToDatesTest()
        {
            var id = DataGenerator.RandomId();
            var personId = DataGenerator.RandomId();
            var date = DataGenerator.RandomDateInYear(2024);
            var percentage = DataGenerator.RandomSPO2Value();
            var json = JsonSerializer.Serialize(new List<dynamic>()
            {
                new
                {
                    Id = id,
                    PersonId = personId,
                    Date = date,
                    Percentage = percentage
                }
            });
            _httpClient.AddResponse(json);

            var from = date.AddDays(-DataGenerator.RandomInt(30, 90));
            var to = date.AddDays(DataGenerator.RandomInt(30, 90));
            var measurements = await _client.ListAsync(personId, from, to, 1, int.MaxValue);

            var encodedFrom = HttpUtility.UrlEncode(from.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(to.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{personId}/{encodedFrom}/{encodedTo}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {_apiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(measurements);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(id, measurements[0].Id);
            Assert.AreEqual(personId, measurements[0].PersonId);
            Assert.AreEqual(date, measurements[0].Date);
            Assert.AreEqual(percentage, measurements[0].Percentage);
        }

        [TestMethod]
        public async Task ImportTest()
        {
            _httpClient.AddResponse("");

            var personId = DataGenerator.RandomId();
            var firstNames = DataGenerator.RandomFirstNames();
            var surname = DataGenerator.RandomSurname();
            var date = DataGenerator.RandomDateInYear(2024);
            var percentage = DataGenerator.RandomSPO2Value();
            var record = $"""{personId}"",""{firstNames} {surname}"",""{date.ToString("dd/MM/yyyy")}"","{percentage}",""Normal""";

            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);
            await _client.ImportFromFileAsync(_filePath);

            Assert.AreEqual($"Bearer {_apiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "ImportBloodOxygenSaturationMeasurement").Route, _httpClient.Requests[0].Uri);
        }

        [TestMethod]
        public async Task ExportTest()
        {
            _httpClient.AddResponse("");

            var personId = DataGenerator.RandomId();
            _filePath = DataGenerator.TemporaryCsvFilePath();

            await _client.ExportAsync(personId, null, null, _filePath);

            Assert.AreEqual($"Bearer {_apiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "ExportBloodOxygenSaturationMeasurement").Route, _httpClient.Requests[0].Uri);
        }

        [TestMethod]
        public async Task CalculateAverageTest()
        {
            var id = DataGenerator.RandomId();
            var personId = DataGenerator.RandomId();
            var to = DataGenerator.RandomDateInYear(2024);
            var from = to.AddDays(-DataGenerator.RandomInt(30, 90));
            var percentage = DataGenerator.RandomSPO2Value();
            var json = JsonSerializer.Serialize(new { Id = id, PersonId = personId, Date = to, Percentage = percentage });
            _httpClient.AddResponse(json);

            var measurement = await _client.CalculateAverageAsync(personId, from, to);

            var encodedFrom = HttpUtility.UrlEncode(from.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(to.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/average/{personId}/{encodedFrom}/{encodedTo}";

            Assert.AreEqual($"Bearer {_apiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(measurement);
            Assert.AreEqual(id, measurement.Id);
            Assert.AreEqual(personId, measurement.PersonId);
            Assert.AreEqual(to, measurement.Date);
            Assert.AreEqual(percentage, measurement.Percentage);
        }

        [TestMethod]
        public async Task CalculateDailyAverageTest()
        {
            var id = DataGenerator.RandomId();
            var personId = DataGenerator.RandomId();
            var to = DataGenerator.RandomDateInYear(2024);
            var from = to.AddDays(-DataGenerator.RandomInt(30, 90));
            var percentage = DataGenerator.RandomSPO2Value();
            var json = JsonSerializer.Serialize(new List<dynamic>()
            {
                new { Id = id, PersonId = personId, Date = to, Percentage = percentage }
            });
            _httpClient.AddResponse(json);

            var measurements = await _client.CalculateDailyAverageAsync(personId, from, to);

            var encodedFrom = HttpUtility.UrlEncode(from.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(to.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/dailyaverage/{personId}/{encodedFrom}/{encodedTo}";

            Assert.AreEqual($"Bearer {_apiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(measurements);
            Assert.AreEqual(id, measurements[0].Id);
            Assert.AreEqual(personId, measurements[0].PersonId);
            Assert.AreEqual(to, measurements[0].Date);
            Assert.AreEqual(percentage, measurements[0].Percentage);
        }

        [TestMethod]
        public async Task ExportDailyAverageTest()
        {
            _httpClient.AddResponse("");

            var personId = DataGenerator.RandomId();
            var to = DataGenerator.RandomDateInYear(2024);
            var from = to.AddDays(-DataGenerator.RandomInt(30, 90));

            _filePath = DataGenerator.TemporaryCsvFilePath();
            await _client.ExportDailyAverageAsync(personId, from, to, _filePath);

            Assert.AreEqual($"Bearer {_apiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "ExportDailyAverageBloodOxygenSaturationRouteKey").Route, _httpClient.Requests[0].Uri);
        }
    }
}

using System.Text.Json;
using System.Web;
using HealthTracker.Client.ApiClient;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.BloodGlucose
{
    [TestClass]
    public class BloodGlucoseMeasurementClientTest
    {
        private readonly string _apiToken = DataGenerator.RandomApiToken();
        private readonly MockHealthTrackerHttpClient _httpClient = new();
        private IBloodGlucoseMeasurementClient _client;
        private string _filePath;

        private readonly HealthTrackerApplicationSettings _settings = new()
        {
            DefaultTimePeriodDays = 7,
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "BloodGlucoseMeasurement", Route = "/bloodglucosemeasurement" },
                new() { Name = "ExportBloodGlucoseMeasurement", Route = "/export/bloodglucosemeasurement" },
                new() { Name = "ImportBloodGlucoseMeasurement", Route = "/import/bloodglucosemeasurement" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(_apiToken);
            _client = new BloodGlucoseMeasurementClient(_httpClient, _settings, provider.Object);
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
            var date = DataGenerator.RandomDateInYear(2024, true);
            var level = DataGenerator.RandomBloodGlucoseValue();
            var json = JsonSerializer.Serialize(new { PersonId = personId, Date = date, Level = level });
            _httpClient.AddResponse(json);

            var measurement = await _client.AddBloodGlucoseMeasurementAsync(personId, date, level);

            Assert.AreEqual($"Bearer {_apiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.AreEqual(json, await _httpClient.Requests[0].Content.ReadAsStringAsync());
            Assert.IsNotNull(measurement);
            Assert.AreEqual(personId, measurement.PersonId);
            Assert.AreEqual(date, measurement.Date);
            Assert.AreEqual(level, measurement.Level);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var id = DataGenerator.RandomId();
            var personId = DataGenerator.RandomId();
            var date = DataGenerator.RandomDateInYear(2024, true);
            var level = DataGenerator.RandomBloodGlucoseValue();
            var json = JsonSerializer.Serialize(new { Id = id, PersonId = personId, Date = date, Level = level });
            _httpClient.AddResponse(json);

            var measurement = await _client.UpdateBloodGlucoseMeasurementAsync(id, personId, date, level);

            Assert.AreEqual($"Bearer {_apiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.AreEqual(json, await _httpClient.Requests[0].Content.ReadAsStringAsync());
            Assert.IsNotNull(measurement);
            Assert.AreEqual(id, measurement.Id);
            Assert.AreEqual(personId, measurement.PersonId);
            Assert.AreEqual(date, measurement.Date);
            Assert.AreEqual(level, measurement.Level);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            var id = DataGenerator.RandomId();
            await _client.DeleteBloodGlucoseMeasurementAsync(id);

            Assert.AreEqual($"Bearer {_apiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Delete, _httpClient.Requests[0].Method);
            Assert.AreEqual($"{_settings.ApiRoutes[0].Route}/{id}", _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
        }

        [TestMethod]
        public async Task ListWithNoDateRangeTest()
        {
            var id = DataGenerator.RandomId();
            var personId = DataGenerator.RandomId();
            var date = DataGenerator.RandomDateInYear(2024, true);
            var level = DataGenerator.RandomBloodGlucoseValue();
            var json = JsonSerializer.Serialize(new List<dynamic>()
            {
                new
                {
                    Id = id,
                    PersonId = personId,
                    Date = date,
                    Level = level
                }
            });
            _httpClient.AddResponse(json);

            var measurements = await _client.ListBloodGlucoseMeasurementsAsync(personId, null, null);

            var expectedTo = DateTime.Now;
            var expectedFrom = expectedTo.AddDays(-_settings.DefaultTimePeriodDays);
            var encodedFrom = HttpUtility.UrlEncode(expectedFrom.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(expectedTo.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{personId}/{encodedFrom}/{encodedTo}";

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
            Assert.AreEqual(level, measurements[0].Level);
        }

        [TestMethod]
        public async Task ListWithFromDateOnlyTest()
        {
            var id = DataGenerator.RandomId();
            var personId = DataGenerator.RandomId();
            var date = DataGenerator.RandomDateInYear(2024, true);
            var level = DataGenerator.RandomBloodGlucoseValue();
            var json = JsonSerializer.Serialize(new List<dynamic>()
            {
                new
                {
                    Id = id,
                    PersonId = personId,
                    Date = date,
                    Level = level
                }
            });
            _httpClient.AddResponse(json);

            var from = date.AddDays(-DataGenerator.RandomInt(30, 90));
            var measurements = await _client.ListBloodGlucoseMeasurementsAsync(personId, from, null);

            var expectedTo = DateTime.Now;
            var encodedFrom = HttpUtility.UrlEncode(from.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(expectedTo.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{personId}/{encodedFrom}/{encodedTo}";

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
            Assert.AreEqual(level, measurements[0].Level);
        }

        [TestMethod]
        public async Task ListWithEndDateOnlyTest()
        {
            var id = DataGenerator.RandomId();
            var personId = DataGenerator.RandomId();
            var date = DataGenerator.RandomDateInYear(2024, true);
            var level = DataGenerator.RandomBloodGlucoseValue();
            var json = JsonSerializer.Serialize(new List<dynamic>()
            {
                new
                {
                    Id = id,
                    PersonId = personId,
                    Date = date,
                    Level = level
                }
            });
            _httpClient.AddResponse(json);

            var to = date.AddDays(DataGenerator.RandomInt(30, 90));
            var measurements = await _client.ListBloodGlucoseMeasurementsAsync(personId, null, to);

            var expectedFrom = to.AddDays(-_settings.DefaultTimePeriodDays);
            var encodedFrom = HttpUtility.UrlEncode(expectedFrom.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(to.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{personId}/{encodedFrom}/{encodedTo}";

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
            Assert.AreEqual(level, measurements[0].Level);
        }

        [TestMethod]
        public async Task ListWithFromAndToDatesTest()
        {
            var id = DataGenerator.RandomId();
            var personId = DataGenerator.RandomId();
            var date = DataGenerator.RandomDateInYear(2024, true);
            var level = DataGenerator.RandomBloodGlucoseValue();
            var json = JsonSerializer.Serialize(new List<dynamic>()
            {
                new
                {
                    Id = id,
                    PersonId = personId,
                    Date = date,
                    Level = level
                }
            });
            _httpClient.AddResponse(json);

            var from = date.AddDays(-DataGenerator.RandomInt(30, 90));
            var to = date.AddDays(DataGenerator.RandomInt(30, 90));
            var measurements = await _client.ListBloodGlucoseMeasurementsAsync(personId, from, to);

            var encodedFrom = HttpUtility.UrlEncode(from.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(to.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{personId}/{encodedFrom}/{encodedTo}";

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
            Assert.AreEqual(level, measurements[0].Level);
        }

        [TestMethod]
        public async Task ImportTest()
        {
            _httpClient.AddResponse("");

            var personId = DataGenerator.RandomId();
            var firstNames = DataGenerator.RandomFirstNames();
            var surname = DataGenerator.RandomSurname();
            var date = DataGenerator.RandomDateInYear(2024, true);
            var percentage = DataGenerator.RandomSPO2Value();
            var record = $"""{personId}"",""{firstNames} {surname}"",""{date.ToString("yyyy-MM-dd HH:mm:ss")}"","{percentage}",""Normal""";

            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);
            await _client.ImportBloodGlucoseMeasurementsAsync(_filePath);

            Assert.AreEqual($"Bearer {_apiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "ImportBloodGlucoseMeasurement").Route, _httpClient.Requests[0].Uri);
        }

        [TestMethod]
        public async Task ExportTest()
        {
            _httpClient.AddResponse("");

            var personId = DataGenerator.RandomId();
            _filePath = DataGenerator.TemporaryCsvFilePath();

            await _client.ExportBloodGlucoseMeasurementsAsync(personId, null, null, _filePath);

            Assert.AreEqual($"Bearer {_apiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "ExportBloodGlucoseMeasurement").Route, _httpClient.Requests[0].Uri);
        }
    }
}

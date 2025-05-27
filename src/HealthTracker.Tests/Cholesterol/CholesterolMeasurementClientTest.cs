using System.Text.Json;
using System.Web;
using HealthTracker.Client.ApiClient;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.Cholesterol
{
    [TestClass]
    public class CholesterolMeasurementClientTest
    {
        private readonly string ApiToken = DataGenerator.RandomApiToken();
        private MockHealthTrackerHttpClient _httpClient = new();
        private ICholesterolMeasurementClient _client;
        private string _filePath;

        private readonly HealthTrackerApplicationSettings _settings = new()
        {
            DefaultTimePeriodDays = 7,
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "CholesterolMeasurement", Route = "/cholesterolmeasurement" },
                new() { Name = "ExportCholesterolMeasurement", Route = "/export/cholesterol" },
                new() { Name = "ImportCholesterolMeasurement", Route = "/import/cholesterol" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(ApiToken);
            _client = new CholesterolMeasurementClient(_httpClient, _settings, provider.Object);
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
            var measurement = DataGenerator.RandomCholesterolMeasurement(personId, 2024);
            var json = JsonSerializer.Serialize(measurement);
            _httpClient.AddResponse(json);

            var added = await _client.AddAsync(personId, measurement.Date, measurement.Total, measurement.HDL, measurement.LDL, measurement.Triglycerides);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.IsNotNull(added);
            Assert.AreEqual(measurement.Id, added.Id);
            Assert.AreEqual(personId, added.PersonId);
            Assert.AreEqual(measurement.Date, added.Date);
            Assert.AreEqual(measurement.Total, added.Total);
            Assert.AreEqual(measurement.HDL, added.HDL);
            Assert.AreEqual(measurement.LDL, added.LDL);
            Assert.AreEqual(measurement.Triglycerides, added.Triglycerides);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var personId = DataGenerator.RandomId();
            var measurement = DataGenerator.RandomCholesterolMeasurement(personId, 2024);
            var json = JsonSerializer.Serialize(measurement);
            _httpClient.AddResponse(json);

            var updated = await _client.UpdateAsync(measurement.Id, personId, measurement.Date, measurement.Total, measurement.HDL, measurement.LDL, measurement.Triglycerides);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.IsNotNull(updated);
            Assert.AreEqual(measurement.Id, updated.Id);
            Assert.AreEqual(personId, updated.PersonId);
            Assert.AreEqual(measurement.Date, updated.Date);
            Assert.AreEqual(measurement.Total, updated.Total);
            Assert.AreEqual(measurement.HDL, updated.HDL);
            Assert.AreEqual(measurement.LDL, updated.LDL);
            Assert.AreEqual(measurement.Triglycerides, updated.Triglycerides);
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
        public async Task ListWithNoDateRangeTest()
        {
            var personId = DataGenerator.RandomId();
            var measurement = DataGenerator.RandomCholesterolMeasurement(personId, 2024);
            var json = JsonSerializer.Serialize(new List<dynamic> { measurement });
            _httpClient.AddResponse(json);

            var measurements = await _client.ListAsync(personId, null, null);

            var expectedTo = DateTime.Now;
            var expectedFrom = expectedTo.AddDays(-_settings.DefaultTimePeriodDays);
            var encodedFrom = HttpUtility.UrlEncode(expectedFrom.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(expectedTo.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{personId}/{encodedFrom}/{encodedTo}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(measurements);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(measurement.Id, measurements[0].Id);
            Assert.AreEqual(personId, measurements[0].PersonId);
            Assert.AreEqual(measurement.Date, measurements[0].Date);
            Assert.AreEqual(measurement.Total, measurements[0].Total);
            Assert.AreEqual(measurement.HDL, measurements[0].HDL);
            Assert.AreEqual(measurement.LDL, measurements[0].LDL);
            Assert.AreEqual(measurement.Triglycerides, measurements[0].Triglycerides);
        }

        [TestMethod]
        public async Task ListWithFromDateOnlyTest()
        {
            var personId = DataGenerator.RandomId();
            var measurement = DataGenerator.RandomCholesterolMeasurement(personId, 2024);
            var json = JsonSerializer.Serialize(new List<dynamic> { measurement });
            _httpClient.AddResponse(json);

            var from = measurement.Date.AddDays(-DataGenerator.RandomInt(30,90));
            var measurements = await _client.ListAsync(personId, from, null);

            var expectedTo = DateTime.Now;
            var encodedFrom = HttpUtility.UrlEncode(from.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(expectedTo.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{personId}/{encodedFrom}/{encodedTo}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(measurements);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(measurement.Id, measurements[0].Id);
            Assert.AreEqual(personId, measurements[0].PersonId);
            Assert.AreEqual(measurement.Date, measurements[0].Date);
            Assert.AreEqual(measurement.Total, measurements[0].Total);
            Assert.AreEqual(measurement.HDL, measurements[0].HDL);
            Assert.AreEqual(measurement.LDL, measurements[0].LDL);
            Assert.AreEqual(measurement.Triglycerides, measurements[0].Triglycerides);
        }

        [TestMethod]
        public async Task ListWithEndDateOnlyTest()
        {
            var personId = DataGenerator.RandomId();
            var measurement = DataGenerator.RandomCholesterolMeasurement(personId, 2024);
            var json = JsonSerializer.Serialize(new List<dynamic> { measurement });
            _httpClient.AddResponse(json);

            var to = measurement.Date.AddDays(DataGenerator.RandomInt(30,90));
            var measurements = await _client.ListAsync(personId, null, to);

            var expectedFrom = to.AddDays(-_settings.DefaultTimePeriodDays);
            var encodedFrom = HttpUtility.UrlEncode(expectedFrom.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(to.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{personId}/{encodedFrom}/{encodedTo}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(measurements);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(measurement.Id, measurements[0].Id);
            Assert.AreEqual(personId, measurements[0].PersonId);
            Assert.AreEqual(measurement.Date, measurements[0].Date);
            Assert.AreEqual(measurement.Total, measurements[0].Total);
            Assert.AreEqual(measurement.HDL, measurements[0].HDL);
            Assert.AreEqual(measurement.LDL, measurements[0].LDL);
            Assert.AreEqual(measurement.Triglycerides, measurements[0].Triglycerides);
        }

        [TestMethod]
        public async Task ListWithFromAndToDatesTest()
        {
            var personId = DataGenerator.RandomId();
            var measurement = DataGenerator.RandomCholesterolMeasurement(personId, 2024);
            var json = JsonSerializer.Serialize(new List<dynamic> { measurement });
            _httpClient.AddResponse(json);

            var from = measurement.Date.AddDays(-DataGenerator.RandomInt(30,90));
            var to = measurement.Date.AddDays(DataGenerator.RandomInt(30,90));
            var measurements = await _client.ListAsync(personId, from, to);

            var encodedFrom = HttpUtility.UrlEncode(from.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(to.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{personId}/{encodedFrom}/{encodedTo}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(measurements);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(measurement.Id, measurements[0].Id);
            Assert.AreEqual(personId, measurements[0].PersonId);
            Assert.AreEqual(measurement.Date, measurements[0].Date);
            Assert.AreEqual(measurement.Total, measurements[0].Total);
            Assert.AreEqual(measurement.HDL, measurements[0].HDL);
            Assert.AreEqual(measurement.LDL, measurements[0].LDL);
            Assert.AreEqual(measurement.Triglycerides, measurements[0].Triglycerides);
        }

        [TestMethod]
        public async Task ImportTest()
        {
            _httpClient.AddResponse("");

            var person = DataGenerator.RandomPerson(16, 90);
            var measurement = DataGenerator.RandomCholesterolMeasurement(person.Id, 2024);
            var record = $"""{person.Id}"",""{person.Name}"",""{measurement.Date:dd-MMM-yyyy HH:mm:ss}"",""{measurement.Total}"",""{measurement.HDL}"",""{measurement.LDL}"",""{measurement.Triglycerides}""";
    
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _client.ImportFromFileAsync(_filePath);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "ImportCholesterolMeasurement").Route, _httpClient.Requests[0].Uri);
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
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "ExportCholesterolMeasurement").Route, _httpClient.Requests[0].Uri);
        }
    }
}
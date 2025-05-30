using System.Text.Json;
using System.Web;
using HealthTracker.Client.ApiClient;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.BloodPressure
{
    [TestClass]
    public class BloodPressureMeasurementClientTest
    {
        private readonly string ApiToken = DataGenerator.RandomApiToken();
        private MockHealthTrackerHttpClient _httpClient = new();
        private IBloodPressureMeasurementClient _client;
        private string _filePath;

        private readonly HealthTrackerApplicationSettings _settings = new()
        {
            DefaultTimePeriodDays = 7,
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "BloodPressureMeasurement", Route = "/bloodpressuremeasurement" },
                new() { Name = "ImportBloodPressureMeasurement", Route = "/export/bloodpressuremeasurement" },
                new() { Name = "ImportOmronBloodPressureMeasurement", Route = "/import/omronbloodpressure" },
                new() { Name = "ExportBloodPressureMeasurement", Route = "/export/bloodpressuremeasurement" },
                new() { Name = "ExportDailyAverageBloodPressureRouteKey", Route = "/export/dailyaveragebloodpressure" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(ApiToken);
            _client = new BloodPressureMeasurementClient(_httpClient, _settings, provider.Object);
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
            var measurement = DataGenerator.RandomBloodPressureMeasurement(personId, 2024, 0, 119, 0, 79);
            var json = JsonSerializer.Serialize(measurement);
            _httpClient.AddResponse(json);

            var added = await _client.AddAsync(personId, measurement.Date, measurement.Systolic, measurement.Diastolic);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "BloodPressureMeasurement").Route, _httpClient.Requests[0].Uri);

            Assert.IsNotNull(added);
            Assert.AreEqual(measurement.Id, added.Id);
            Assert.AreEqual(personId, added.PersonId);
            Assert.AreEqual(measurement.Date, added.Date);
            Assert.AreEqual(measurement.Systolic, added.Systolic);
            Assert.AreEqual(measurement.Diastolic, added.Diastolic);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var personId = DataGenerator.RandomId();
            var measurement = DataGenerator.RandomBloodPressureMeasurement(personId, 2024, 0, 119, 0, 79);
            var json = JsonSerializer.Serialize(measurement);
            _httpClient.AddResponse(json);

            var updated = await _client.UpdateAsync(measurement.Id, personId, measurement.Date, measurement.Systolic, measurement.Diastolic);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "BloodPressureMeasurement").Route, _httpClient.Requests[0].Uri);

            Assert.IsNotNull(updated);
            Assert.AreEqual(measurement.Id, updated.Id);
            Assert.AreEqual(personId, updated.PersonId);
            Assert.AreEqual(measurement.Date, updated.Date);
            Assert.AreEqual(measurement.Systolic, updated.Systolic);
            Assert.AreEqual(measurement.Diastolic, updated.Diastolic);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            var id = DataGenerator.RandomId();
            await _client.DeleteAsync(id);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Delete, _httpClient.Requests[0].Method);
            Assert.AreEqual($"{_settings.ApiRoutes.First(x => x.Name == "BloodPressureMeasurement").Route}/{id}", _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
        }

        [TestMethod]
        public async Task GetTest()
        {
            var personId = DataGenerator.RandomId();
            var measurement = DataGenerator.RandomBloodPressureMeasurement(personId, 2024, 100, 130, 70, 80);
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
            Assert.AreEqual(personId, retrieved.PersonId);
            Assert.AreEqual(measurement.Date, retrieved.Date);
            Assert.AreEqual(measurement.Systolic, retrieved.Systolic);
            Assert.AreEqual(measurement.Diastolic, retrieved.Diastolic);
        }

        [TestMethod]
        public async Task ListWithNoDateRangeTest()
        {
            var personId = DataGenerator.RandomId();
            var measurement = DataGenerator.RandomBloodPressureMeasurement(personId, 2024, 0, 119, 0, 79);
            var json = JsonSerializer.Serialize(new List<dynamic>() { measurement } );
            _httpClient.AddResponse(json);

            var measurements = await _client.ListAsync(personId, null, null, 1, int.MaxValue);

            var expectedTo = DateTime.Now;
            var expectedFrom = expectedTo.AddDays(-_settings.DefaultTimePeriodDays);
            var encodedFrom = HttpUtility.UrlEncode(expectedFrom.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(expectedTo.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes.First(x => x.Name == "BloodPressureMeasurement").Route}/{personId}/{encodedFrom}/{encodedTo}/1/{int.MaxValue}";

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
            Assert.AreEqual(measurement.Systolic, measurements[0].Systolic);
            Assert.AreEqual(measurement.Diastolic, measurements[0].Diastolic);
        }

        [TestMethod]
        public async Task ListWithFromDateOnlyTest()
        {
            var personId = DataGenerator.RandomId();
            var measurement = DataGenerator.RandomBloodPressureMeasurement(personId, 2024, 0, 119, 0, 79);
            var json = JsonSerializer.Serialize(new List<dynamic>() { measurement } );
            _httpClient.AddResponse(json);

            var from = measurement.Date.AddDays(-DataGenerator.RandomInt(30, 90));
            var measurements = await _client.ListAsync(personId, from, null, 1, int.MaxValue);

            var expectedTo = DateTime.Now;
            var encodedFrom = HttpUtility.UrlEncode(from.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(expectedTo.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes.First(x => x.Name == "BloodPressureMeasurement").Route}/{personId}/{encodedFrom}/{encodedTo}/1/{int.MaxValue}";

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
            Assert.AreEqual(measurement.Systolic, measurements[0].Systolic);
            Assert.AreEqual(measurement.Diastolic, measurements[0].Diastolic);
        }

        [TestMethod]
        public async Task ListWithEndDateOnlyTest()
        {
            var personId = DataGenerator.RandomId();
            var measurement = DataGenerator.RandomBloodPressureMeasurement(personId, 2024, 0, 119, 0, 79);
            var json = JsonSerializer.Serialize(new List<dynamic>() { measurement } );
            _httpClient.AddResponse(json);

            var to = measurement.Date.AddDays(DataGenerator.RandomInt(30, 90));
            var measurements = await _client.ListAsync(personId, null, to, 1, int.MaxValue);

            var expectedFrom = to.AddDays(-_settings.DefaultTimePeriodDays);
            var encodedFrom = HttpUtility.UrlEncode(expectedFrom.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(to.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes.First(x => x.Name == "BloodPressureMeasurement").Route}/{personId}/{encodedFrom}/{encodedTo}/1/{int.MaxValue}";

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
            Assert.AreEqual(measurement.Systolic, measurements[0].Systolic);
            Assert.AreEqual(measurement.Diastolic, measurements[0].Diastolic);
        }

        [TestMethod]
        public async Task ListWithFromAndToDatesTest()
        {
            var personId = DataGenerator.RandomId();
            var measurement = DataGenerator.RandomBloodPressureMeasurement(personId, 2024, 0, 119, 0, 79);
            var json = JsonSerializer.Serialize(new List<dynamic>() { measurement } );
            _httpClient.AddResponse(json);

            var from = measurement.Date.AddDays(-DataGenerator.RandomInt(30,90));
            var to = measurement.Date.AddDays(DataGenerator.RandomInt(30,90));
            var measurements = await _client.ListAsync(personId, from, to, 1, int.MaxValue);

            var encodedFrom = HttpUtility.UrlEncode(from.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(to.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes.First(x => x.Name == "BloodPressureMeasurement").Route}/{personId}/{encodedFrom}/{encodedTo}/1/{int.MaxValue}";

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
            Assert.AreEqual(measurement.Systolic, measurements[0].Systolic);
            Assert.AreEqual(measurement.Diastolic, measurements[0].Diastolic);
        }

        [TestMethod]
        public async Task ImportTest()
        {
            _httpClient.AddResponse("");

            var person = DataGenerator.RandomPerson(16, 90);
            var measurement = DataGenerator.RandomBloodPressureMeasurement(person.Id, 2024, 0, 119, 0, 79);
            var record = $"""{person.Id}"",""{person.Name}"",""{measurement.Date.ToString("dd/MM/yyyy")}"",""{measurement.Systolic}"",""{measurement.Diastolic}"",""Optimal""";

            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _client.ImportFromFileAsync(_filePath);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "ImportBloodPressureMeasurement").Route, _httpClient.Requests[0].Uri);
        }

        [TestMethod]
        public async Task ImportOmronTest()
        {
            _httpClient.AddResponse("");

            await _client.ImportOmronAsync("omron.xlsx", DataGenerator.RandomId());

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "ImportOmronBloodPressureMeasurement").Route, _httpClient.Requests[0].Uri);
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
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "ExportBloodPressureMeasurement").Route, _httpClient.Requests[0].Uri);
        }

        [TestMethod]
        public async Task CalculateAverageTest()
        {
            var personId = DataGenerator.RandomId();
            var measurement = DataGenerator.RandomBloodPressureMeasurement(personId, 2024, 0, 119, 0, 79);
            var from = measurement.Date.AddDays(-DataGenerator.RandomInt(30, 90));
            var to = measurement.Date.AddDays(DataGenerator.RandomInt(30, 90));
            var json = JsonSerializer.Serialize(measurement);
            _httpClient.AddResponse(json);

            var average = await _client.CalculateAverageAsync(personId, from, to);

            var encodedFrom = HttpUtility.UrlEncode(from.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(to.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes.First(x => x.Name == "BloodPressureMeasurement").Route}/average/{personId}/{encodedFrom}/{encodedTo}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(average);
            Assert.AreEqual(measurement.Id, average.Id);
            Assert.AreEqual(personId, average.PersonId);
            Assert.AreEqual(measurement.Date, average.Date);
            Assert.AreEqual(measurement.Systolic, average.Systolic);
            Assert.AreEqual(measurement.Diastolic, average.Diastolic);
        }

        [TestMethod]
        public async Task CalculateDailyAverageTest()
        {
            var personId = DataGenerator.RandomId();
            var measurement = DataGenerator.RandomBloodPressureMeasurement(personId, 2024, 0, 119, 0, 79);
            var from = measurement.Date.AddDays(-DataGenerator.RandomInt(30, 90));
            var to = measurement.Date.AddDays(DataGenerator.RandomInt(30, 90));
            var json = JsonSerializer.Serialize(new List<dynamic> { measurement });
            _httpClient.AddResponse(json);

            var measurements = await _client.CalculateDailyAverageAsync(personId, from, to);

            var encodedFrom = HttpUtility.UrlEncode(from.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(to.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes.First(x => x.Name == "BloodPressureMeasurement").Route}/dailyaverage/{personId}/{encodedFrom}/{encodedTo}";

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
            Assert.AreEqual(measurement.Systolic, measurements[0].Systolic);
            Assert.AreEqual(measurement.Diastolic, measurements[0].Diastolic);
        }

        [TestMethod]
        public async Task ExportDailyAverageTest()
        {
            _httpClient.AddResponse("");

            var personId = DataGenerator.RandomId();
            var to = DateTime.Now;
            var from = to.AddDays(-DataGenerator.RandomInt(30, 90));

            _filePath = DataGenerator.TemporaryCsvFilePath();
            var json = JsonSerializer.Serialize(new { PersonId = personId, From = from, To = to, FileName = _filePath });

            await _client.ExportDailyAverageAsync(personId, from, to, _filePath);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "ExportDailyAverageBloodPressureRouteKey").Route, _httpClient.Requests[0].Uri);
        }
    }
}
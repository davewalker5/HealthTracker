using System.Text.Json;
using System.Web;
using HealthTracker.Client.ApiClient;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.Entities.Measurements;
using HealthTracker.Tests.Mocks;
using Microsoft.Extensions.Logging;
using Moq;

namespace HealthTracker.Tests.BeverageConsumption
{
    [TestClass]
    public class BeverageConsumptionMeasurementClientTest
    {
        private readonly string ApiToken = DataGenerator.RandomApiToken();
        private MockHealthTrackerHttpClient _httpClient = new();
        private IBeverageConsumptionMeasurementClient _client;
        private string _filePath;

        private readonly HealthTrackerApplicationSettings _settings = new()
        {
            DefaultTimePeriodDays = 7,
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "BeverageConsumptionMeasurement", Route = "/beverageconsumptionmeasurement" },
                new() { Name = "ExportBeverageConsumptionMeasurement", Route = "/export/beverageconsumption" },
                new() { Name = "ImportBeverageConsumptionMeasurement", Route = "/import/beverageconsumption" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(ApiToken);
            var logger = new Mock<ILogger<BeverageConsumptionMeasurementClient>>();
            _client = new BeverageConsumptionMeasurementClient(_httpClient, _settings, provider.Object, logger.Object);
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
            var measurement = DataGenerator.RandomBeverageConsumptionMeasurement(2024);
            var json = JsonSerializer.Serialize(measurement);
            _httpClient.AddResponse(json);

            var added = await _client.AddAsync(
                measurement.PersonId,
                measurement.BeverageId,
                measurement.Date,
                measurement.Quantity,
                measurement.Volume,
                measurement.ABV);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.IsNotNull(added);
            Assert.AreEqual(measurement.Id, added.Id);
            Assert.AreEqual(measurement.PersonId, added.PersonId);
            Assert.AreEqual(measurement.Date, added.Date);
            Assert.AreEqual(measurement.BeverageId, added.BeverageId);
            Assert.AreEqual(measurement.Volume, added.Volume);
            Assert.AreEqual(measurement.Quantity, added.Quantity);
            Assert.AreEqual(measurement.ABV, added.ABV);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var measurement = DataGenerator.RandomBeverageConsumptionMeasurement(2024);
            var json = JsonSerializer.Serialize(measurement);
            _httpClient.AddResponse(json);

            var updated = await _client.UpdateAsync(
                measurement.Id,
                measurement.PersonId,
                measurement.BeverageId,
                measurement.Date,
                measurement.Quantity,
                measurement.Volume,
                measurement.ABV);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.IsNotNull(updated);
            Assert.AreEqual(measurement.Id, updated.Id);
            Assert.AreEqual(measurement.PersonId, updated.PersonId);
            Assert.AreEqual(measurement.Date, updated.Date);
            Assert.AreEqual(measurement.BeverageId, updated.BeverageId);
            Assert.AreEqual(measurement.Volume, updated.Volume);
            Assert.AreEqual(measurement.Quantity, updated.Quantity);
            Assert.AreEqual(measurement.ABV, updated.ABV);
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
            var measurement = DataGenerator.RandomBeverageConsumptionMeasurement(2024);
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
            Assert.AreEqual(measurement.BeverageId, retrieved.BeverageId);
            Assert.AreEqual(measurement.Volume, retrieved.Volume);
            Assert.AreEqual(measurement.Quantity, retrieved.Quantity);
            Assert.AreEqual(measurement.ABV, retrieved.ABV);
        }

        [TestMethod]
        public async Task ListWithNoDateRangeTest()
        {
            var measurement = DataGenerator.RandomBeverageConsumptionMeasurement(2024);
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
            Assert.AreEqual(measurement.BeverageId, measurements[0].BeverageId);
            Assert.AreEqual(measurement.Volume, measurements[0].Volume);
            Assert.AreEqual(measurement.Quantity, measurements[0].Quantity);
            Assert.AreEqual(measurement.ABV, measurements[0].ABV);
        }

        [TestMethod]
        public async Task ListWithFromDateOnlyTest()
        {
            var measurement = DataGenerator.RandomBeverageConsumptionMeasurement(2024);
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
            Assert.AreEqual(measurement.BeverageId, measurements[0].BeverageId);
            Assert.AreEqual(measurement.Volume, measurements[0].Volume);
            Assert.AreEqual(measurement.Quantity, measurements[0].Quantity);
            Assert.AreEqual(measurement.ABV, measurements[0].ABV);
        }

        [TestMethod]
        public async Task ListWithEndDateOnlyTest()
        {
            var measurement = DataGenerator.RandomBeverageConsumptionMeasurement(2024);
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
            Assert.AreEqual(measurement.BeverageId, measurements[0].BeverageId);
            Assert.AreEqual(measurement.Volume, measurements[0].Volume);
            Assert.AreEqual(measurement.Quantity, measurements[0].Quantity);
            Assert.AreEqual(measurement.ABV, measurements[0].ABV);
        }

        [TestMethod]
        public async Task ListWithFromAndToDatesTest()
        {
            var measurement = DataGenerator.RandomBeverageConsumptionMeasurement(2024);
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
            Assert.AreEqual(measurement.BeverageId, measurements[0].BeverageId);
            Assert.AreEqual(measurement.Volume, measurements[0].Volume);
            Assert.AreEqual(measurement.Quantity, measurements[0].Quantity);
            Assert.AreEqual(measurement.ABV, measurements[0].ABV);
        }

        [TestMethod]
        public async Task ImportTest()
        {
            _httpClient.AddResponse("");

            var person = DataGenerator.RandomPerson(16, 90);
            var beverage = DataGenerator.RandomBeverage();
            var measurement = DataGenerator.RandomBeverageConsumptionMeasurement(person.Id, beverage.Id, 2024);
            var record = $"""{measurement.PersonId}"",""{person.Name}"",""{measurement.Date:dd-MMM-yyyy HH:mm:ss}"",""{beverage.Id}"",""{beverage.Name}"",""{measurement.Quantity}"",""{measurement.Volume}"",""{measurement.ABV}""";

            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _client.ImportFromFileAsync(_filePath);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "ImportBeverageConsumptionMeasurement").Route, _httpClient.Requests[0].Uri);
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
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "ExportBeverageConsumptionMeasurement").Route, _httpClient.Requests[0].Uri);
        }

        [TestMethod]
        public async Task CalculateTotalHydratingTest()
        {
            var person = DataGenerator.RandomPerson(16, 90);
            var beverage = DataGenerator.RandomBeverage(true, false);
            var summary = DataGenerator.RandomBeverageConsumptionSummary(person.Id, beverage.Id, 2024);
            var json = JsonSerializer.Serialize(summary);
            _httpClient.AddResponse(json);

            var expectedTo = DateTime.Today.AddDays(1).AddSeconds(-1);
            var total = await _client.CalculateTotalHydratingAsync(person.Id, _settings.DefaultTimePeriodDays);
            var expectedFrom = DateTime.Today.AddDays(-_settings.DefaultTimePeriodDays + 1);
            var encodedFrom = HttpUtility.UrlEncode(expectedFrom.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(expectedTo.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/totalhydrating/{person.Id}/{encodedFrom}/{encodedTo}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(total);
            Assert.AreEqual(summary.PersonId, total.PersonId);
            Assert.AreEqual(summary.PersonName, total.PersonName);
            Assert.AreEqual(summary.From, total.From);
            Assert.AreEqual(summary.To, total.To);
            Assert.AreEqual(summary.BeverageId, total.BeverageId);
            Assert.AreEqual(summary.BeverageName, total.BeverageName);
            Assert.AreEqual(summary.TotalVolume, total.TotalVolume);
            Assert.AreEqual(summary.TotalUnits, total.TotalUnits);
        }

        [TestMethod]
        public async Task CalculateDailyTotalHydratingTest()
        {
            var person = DataGenerator.RandomPerson(16, 90);
            var beverage = DataGenerator.RandomBeverage(true, false);
            var summary = DataGenerator.RandomBeverageConsumptionSummary(person.Id, beverage.Id, 2024);
            var json = JsonSerializer.Serialize<List<BeverageConsumptionSummary>>([summary]);
            _httpClient.AddResponse(json);

            var from = DateTime.Today.AddDays(-DataGenerator.RandomInt(1, 10));
            var to = DateTime.Today.AddDays(DataGenerator.RandomInt(1, 10));

            var totals = await _client.CalculateDailyTotalHydratingAsync(person.Id, from, to);
            var encodedFrom = HttpUtility.UrlEncode(from.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(to.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/dailytotalhydrating/{person.Id}/{encodedFrom}/{encodedTo}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(totals);
            Assert.AreEqual(1, totals.Count);
            Assert.AreEqual(summary.PersonId, totals[0].PersonId);
            Assert.AreEqual(summary.PersonName, totals[0].PersonName);
            Assert.AreEqual(summary.From, totals[0].From);
            Assert.AreEqual(summary.To, totals[0].To);
            Assert.AreEqual(summary.BeverageId, totals[0].BeverageId);
            Assert.AreEqual(summary.BeverageName, totals[0].BeverageName);
            Assert.AreEqual(summary.TotalVolume, totals[0].TotalVolume);
            Assert.AreEqual(summary.TotalUnits, totals[0].TotalUnits);
        }

        [TestMethod]
        public async Task CalculateTotalAlcoholTest()
        {
            var person = DataGenerator.RandomPerson(16, 90);
            var beverage = DataGenerator.RandomBeverage(false, true);
            var summary = DataGenerator.RandomBeverageConsumptionSummary(person.Id, beverage.Id, 2024);
            var json = JsonSerializer.Serialize(summary);
            _httpClient.AddResponse(json);

            var expectedTo = DateTime.Today.AddDays(1).AddSeconds(-1);
            var total = await _client.CalculateTotalAlcoholicAsync(person.Id, _settings.DefaultTimePeriodDays);
            var expectedFrom = DateTime.Today.AddDays(-_settings.DefaultTimePeriodDays + 1);
            var encodedFrom = HttpUtility.UrlEncode(expectedFrom.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(expectedTo.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/totalalcohol/{person.Id}/{encodedFrom}/{encodedTo}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(total);
            Assert.AreEqual(summary.PersonId, total.PersonId);
            Assert.AreEqual(summary.PersonName, total.PersonName);
            Assert.AreEqual(summary.From, total.From);
            Assert.AreEqual(summary.To, total.To);
            Assert.AreEqual(summary.BeverageId, total.BeverageId);
            Assert.AreEqual(summary.BeverageName, total.BeverageName);
            Assert.AreEqual(summary.TotalVolume, total.TotalVolume);
            Assert.AreEqual(summary.TotalUnits, total.TotalUnits);
        }

        [TestMethod]
        public async Task CalculateDailyTotalAlcoholTest()
        {
            var person = DataGenerator.RandomPerson(16, 90);
            var beverage = DataGenerator.RandomBeverage(true, false);
            var summary = DataGenerator.RandomBeverageConsumptionSummary(person.Id, beverage.Id, 2024);
            var json = JsonSerializer.Serialize<List<BeverageConsumptionSummary>>([summary]);
            _httpClient.AddResponse(json);

            var from = DateTime.Today.AddDays(-DataGenerator.RandomInt(1, 10));
            var to = DateTime.Today.AddDays(DataGenerator.RandomInt(1, 10));

            var totals = await _client.CalculateDailyTotalAlcoholicAsync(person.Id, from, to);
            var encodedFrom = HttpUtility.UrlEncode(from.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(to.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/dailytotalalcohol/{person.Id}/{encodedFrom}/{encodedTo}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(totals);
            Assert.AreEqual(1, totals.Count);
            Assert.AreEqual(summary.PersonId, totals[0].PersonId);
            Assert.AreEqual(summary.PersonName, totals[0].PersonName);
            Assert.AreEqual(summary.From, totals[0].From);
            Assert.AreEqual(summary.To, totals[0].To);
            Assert.AreEqual(summary.BeverageId, totals[0].BeverageId);
            Assert.AreEqual(summary.BeverageName, totals[0].BeverageName);
            Assert.AreEqual(summary.TotalVolume, totals[0].TotalVolume);
            Assert.AreEqual(summary.TotalUnits, totals[0].TotalUnits);
        }
    }
}
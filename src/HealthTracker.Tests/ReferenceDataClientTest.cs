using System.Text.Json;
using HealthTracker.Client.ApiClient;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.Entities.Measurements;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.ActivityTypes
{
    [TestClass]
    public class ReferenceDataClientTest
    {
        private readonly string ApiToken = DataGenerator.RandomApiToken();
        private readonly MockHealthTrackerHttpClient _httpClient = new();
        private IReferenceDataClient _client;

        private readonly HealthTrackerApplicationSettings _settings = new()
        {
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "ReferenceData", Route = "/referencedata" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(ApiToken);
            _client = new ReferenceDataClient(_httpClient, _settings, provider.Object);
        }

        [TestMethod]
        public async Task ListBloodPressureBandsTest()
        {
            var band = DataGenerator.RandomBloodPressureBand();
            var json = JsonSerializer.Serialize<List<BloodPressureBand>>([band]);
            _httpClient.AddResponse(json);

            var bands = await _client.ListBloodPressureAssessmentBandsAsync();
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/bloodpressurebands";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(bands);
            Assert.AreEqual(1, bands.Count);
            Assert.AreEqual(band.Id, bands[0].Id);
        }

        [TestMethod]
        public async Task ListBloodOxygenBandsTest()
        {
            var band = DataGenerator.RandomBloodOxygenSaturationBand();
            var json = JsonSerializer.Serialize<List<BloodOxygenSaturationBand>>([band]);
            _httpClient.AddResponse(json);

            var bands = await _client.ListBloodOxygenSaturationAssessmentBandsAsync();
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/spo2bands";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(bands);
            Assert.AreEqual(1, bands.Count);
            Assert.AreEqual(band.Id, bands[0].Id);
        }

        [TestMethod]
        public async Task ListBMIBandsTest()
        {
            var band = DataGenerator.RandomBMIBand();
            var json = JsonSerializer.Serialize<List<BMIBand>>([band]);
            _httpClient.AddResponse(json);

            var bands = await _client.ListBMIAssessmentBandsAsync();
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/bmibands";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(bands);
            Assert.AreEqual(1, bands.Count);
            Assert.AreEqual(band.Id, bands[0].Id);
        }
    }
}

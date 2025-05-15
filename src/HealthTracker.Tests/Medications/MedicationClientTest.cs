using System.Text.Json;
using HealthTracker.Client.ApiClient;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.Medications
{
    [TestClass]
    public class MedicationClientTest
    {
        private readonly string ApiToken = DataGenerator.RandomApiToken();
        private MockHealthTrackerHttpClient _httpClient = new();
        private IMedicationClient _client;

        private readonly HealthTrackerApplicationSettings _settings = new()
        {
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "Medication", Route = "/medication" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(ApiToken);
            _client = new MedicationClient(_httpClient, _settings, provider.Object);
        }

        [TestMethod]
        public async Task AddTest()
        {
            var name = DataGenerator.RandomMedicationName();
            var json = JsonSerializer.Serialize(new { Name = name });
            _httpClient.AddResponse(json);

            var medication = await _client.AddMedicationAsync(name);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.AreEqual(json, await _httpClient.Requests[0].Content.ReadAsStringAsync());
            Assert.IsNotNull(medication);
            Assert.AreEqual(name, medication.Name);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var medication = DataGenerator.RandomMedication();
            var json = JsonSerializer.Serialize(medication);
            _httpClient.AddResponse(json);

            var updated = await _client.UpdateMedicationAsync(medication.Id, medication.Name);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.IsNotNull(updated);
            Assert.AreEqual(medication.Id, updated.Id);
            Assert.AreEqual(medication.Name, updated.Name);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            var id = DataGenerator.RandomId();
            await _client.DeleteMedicationAsync(id);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Delete, _httpClient.Requests[0].Method);
            Assert.AreEqual($"{_settings.ApiRoutes[0].Route}/{id}", _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
        }

        [TestMethod]
        public async Task ListTest()
        {
            var medication = DataGenerator.RandomMedication();
            var json = JsonSerializer.Serialize(new List<dynamic>() { medication });
            _httpClient.AddResponse(json);

            var medications = await _client.ListMedicationsAsync(1, int.MaxValue);
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(medications);
            Assert.AreEqual(1, medications.Count);
            Assert.AreEqual(medication.Id, medications[0].Id);
            Assert.AreEqual(medication.Name, medications[0].Name);
        }
    }
}

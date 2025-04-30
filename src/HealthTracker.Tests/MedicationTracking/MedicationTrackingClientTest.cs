using System.Text.Json;
using HealthTracker.Client.ApiClient;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.MedicationTracking
{
    [TestClass]
    public class MedicationTrackingClientTest
    {
        private readonly string ApiToken = DataGenerator.RandomApiToken();
        private MockHealthTrackerHttpClient _httpClient = new();
        private IMedicationTrackingClient _client;

        private readonly HealthTrackerApplicationSettings _settings = new()
        {
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "MedicationTracking", Route = "/medicationtracking" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(ApiToken);
            _client = new MedicationTrackingClient(_httpClient, _settings, provider.Object);
        }

        [TestMethod]
        public async Task SetStockTest()
        {
            var association = DataGenerator.RandomPersonMedicationAssociation();
            var json = JsonSerializer.Serialize(association);
            _httpClient.AddResponse(json);

            var updated = await _client.SetMedicationStockAsync(association.PersonId, association.MedicationId, association.Stock);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual($"{_settings.ApiRoutes[0].Route}/setstock", _httpClient.Requests[0].Uri);

            Assert.IsNotNull(updated);
            Assert.AreEqual(association.Id, updated.Id);
            Assert.AreEqual(association.PersonId, updated.PersonId);
            Assert.AreEqual(association.MedicationId, updated.MedicationId);
            Assert.AreEqual(association.DailyDose, updated.DailyDose);
            Assert.AreEqual(association.Stock, updated.Stock);
            Assert.AreEqual(association.LastTaken, updated.LastTaken);
        }

        [TestMethod]
        public async Task AddStockTest()
        {
            var association = DataGenerator.RandomPersonMedicationAssociation();
            var json = JsonSerializer.Serialize(association);
            _httpClient.AddResponse(json);

            var tabletsToAdd = DataGenerator.RandomInt(1, 100);
            var updated = await _client.AddMedicationStockAsync(association.PersonId, association.MedicationId, association.Stock);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual($"{_settings.ApiRoutes[0].Route}/addstock", _httpClient.Requests[0].Uri);

            Assert.IsNotNull(updated);
            Assert.AreEqual(association.Id, updated.Id);
            Assert.AreEqual(association.PersonId, updated.PersonId);
            Assert.AreEqual(association.MedicationId, updated.MedicationId);
            Assert.AreEqual(association.DailyDose, updated.DailyDose);
            Assert.AreEqual(association.Stock, updated.Stock);
            Assert.AreEqual(association.LastTaken, updated.LastTaken);
        }

        [TestMethod]
        public async Task SetDoseTest()
        {
            var association = DataGenerator.RandomPersonMedicationAssociation();
            var json = JsonSerializer.Serialize(association);
            _httpClient.AddResponse(json);

            var updated = await _client.SetMedicationDoseAsync(association.PersonId, association.MedicationId, association.DailyDose);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual($"{_settings.ApiRoutes[0].Route}/setdose", _httpClient.Requests[0].Uri);

            Assert.IsNotNull(updated);
            Assert.AreEqual(association.Id, updated.Id);
            Assert.AreEqual(association.PersonId, updated.PersonId);
            Assert.AreEqual(association.MedicationId, updated.MedicationId);
            Assert.AreEqual(association.DailyDose, updated.DailyDose);
            Assert.AreEqual(association.Stock, updated.Stock);
            Assert.AreEqual(association.LastTaken, updated.LastTaken);
        }

        [TestMethod]
        public async Task TakeDoseTest()
        {
            var association = DataGenerator.RandomPersonMedicationAssociation();
            var json = JsonSerializer.Serialize(association);
            _httpClient.AddResponse(json);

            var updated = await _client.TakeDoseAsync(association.PersonId, association.MedicationId);
            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual($"{_settings.ApiRoutes[0].Route}/take", _httpClient.Requests[0].Uri);

            Assert.IsNotNull(updated);
            Assert.AreEqual(association.Id, updated.Id);
            Assert.AreEqual(association.PersonId, updated.PersonId);
            Assert.AreEqual(association.MedicationId, updated.MedicationId);
            Assert.AreEqual(association.DailyDose, updated.DailyDose);
            Assert.AreEqual(association.Stock, updated.Stock);
            Assert.AreEqual(association.LastTaken, updated.LastTaken);
        }

        [TestMethod]
        public async Task TakeAllDosesTest()
        {
            _httpClient.AddResponse("");

            var personId = DataGenerator.RandomId();
            await _client.TakeAllDosesAsync(personId);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual($"{_settings.ApiRoutes[0].Route}/takeall", _httpClient.Requests[0].Uri);

            var expectedRequestJson = JsonSerializer.Serialize(new { PersonId = personId });
            Assert.AreEqual(expectedRequestJson, await _httpClient.Requests[0].Content.ReadAsStringAsync());
        }

        [TestMethod]
        public async Task UntakeDoseTest()
        {
            var association = DataGenerator.RandomPersonMedicationAssociation();
            var json = JsonSerializer.Serialize(association);
            _httpClient.AddResponse(json);

            var updated = await _client.UntakeDoseAsync(association.PersonId, association.MedicationId);
            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual($"{_settings.ApiRoutes[0].Route}/untake", _httpClient.Requests[0].Uri);

            var expectedRequestJson = JsonSerializer.Serialize(new { association.PersonId, association.MedicationId });
            Assert.AreEqual(expectedRequestJson, await _httpClient.Requests[0].Content.ReadAsStringAsync());
            Assert.IsNotNull(updated);
            Assert.AreEqual(association.Id, updated.Id);
            Assert.AreEqual(association.PersonId, updated.PersonId);
            Assert.AreEqual(association.MedicationId, updated.MedicationId);
            Assert.AreEqual(association.DailyDose, updated.DailyDose);
            Assert.AreEqual(association.Stock, updated.Stock);
            Assert.AreEqual(association.LastTaken, updated.LastTaken);
        }

        [TestMethod]
        public async Task UntakeAllDosesTest()
        {
            _httpClient.AddResponse("");

            var personId = DataGenerator.RandomId();
            await _client.UntakeAllDosesAsync(personId);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual($"{_settings.ApiRoutes[0].Route}/untakeall", _httpClient.Requests[0].Uri);

            var expectedRequestJson = JsonSerializer.Serialize(new { PersonId = personId });
            Assert.AreEqual(expectedRequestJson, await _httpClient.Requests[0].Content.ReadAsStringAsync());
        }

        [TestMethod]
        public async Task FastForwardTest()
        {
            var association = DataGenerator.RandomPersonMedicationAssociation();
            var json = JsonSerializer.Serialize(association);
            _httpClient.AddResponse(json);

            var updated = await _client.FastForwardAsync(association.PersonId, association.MedicationId);
            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual($"{_settings.ApiRoutes[0].Route}/fastforward", _httpClient.Requests[0].Uri);

            var expectedRequestJson = JsonSerializer.Serialize(new { association.PersonId, association.MedicationId });
            Assert.AreEqual(expectedRequestJson, await _httpClient.Requests[0].Content.ReadAsStringAsync());
            Assert.IsNotNull(updated);
            Assert.AreEqual(association.Id, updated.Id);
            Assert.AreEqual(association.PersonId, updated.PersonId);
            Assert.AreEqual(association.MedicationId, updated.MedicationId);
            Assert.AreEqual(association.DailyDose, updated.DailyDose);
            Assert.AreEqual(association.Stock, updated.Stock);
            Assert.AreEqual(association.LastTaken, updated.LastTaken);
        }

        [TestMethod]
        public async Task FastForwardAllTest()
        {
            var personId = 346;
            _httpClient.AddResponse("");

            await _client.FastForwardAllAsync(personId);
            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual($"{_settings.ApiRoutes[0].Route}/fastforwardall", _httpClient.Requests[0].Uri);

            var expectedRequestJson = JsonSerializer.Serialize(new { PersonId = personId });
            Assert.AreEqual(expectedRequestJson, await _httpClient.Requests[0].Content.ReadAsStringAsync());
        }

        [TestMethod]
        public async Task SkipDoseTest()
        {
            var association = DataGenerator.RandomPersonMedicationAssociation();
            var json = JsonSerializer.Serialize(association);
            _httpClient.AddResponse(json);

            var updated = await _client.SkipDoseAsync(association.PersonId, association.MedicationId);
            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual($"{_settings.ApiRoutes[0].Route}/skip", _httpClient.Requests[0].Uri);

            var expectedRequestJson = JsonSerializer.Serialize(new { association.PersonId, association.MedicationId });
            Assert.AreEqual(expectedRequestJson, await _httpClient.Requests[0].Content.ReadAsStringAsync());
            Assert.IsNotNull(updated);
            Assert.AreEqual(association.Id, updated.Id);
            Assert.AreEqual(association.PersonId, updated.PersonId);
            Assert.AreEqual(association.MedicationId, updated.MedicationId);
            Assert.AreEqual(association.DailyDose, updated.DailyDose);
            Assert.AreEqual(association.Stock, updated.Stock);
            Assert.AreEqual(association.LastTaken, updated.LastTaken);
        }

        [TestMethod]
        public async Task SkipAllDosesTest()
        {
            _httpClient.AddResponse("");

            var personId = DataGenerator.RandomId();
            await _client.SkipAllDosesAsync(personId);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual($"{_settings.ApiRoutes[0].Route}/skipall", _httpClient.Requests[0].Uri);

            var expectedRequestJson = JsonSerializer.Serialize(new { PersonId = personId });
            Assert.AreEqual(expectedRequestJson, await _httpClient.Requests[0].Content.ReadAsStringAsync());
        }
    }
}
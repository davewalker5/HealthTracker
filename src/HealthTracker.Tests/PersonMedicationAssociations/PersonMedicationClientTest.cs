using System.Text.Json;
using HealthTracker.Client.ApiClient;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.PersonMedicationAssociations
{
    [TestClass]
    public class PersonMedicationClientTest
    {
        private readonly string ApiToken = DataGenerator.RandomApiToken();
        private MockHealthTrackerHttpClient _httpClient = new();
        private IPersonMedicationClient _client;

        private readonly HealthTrackerApplicationSettings _settings = new()
        {
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "PersonMedication", Route = "/personmedication" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(ApiToken);
            _client = new PersonMedicationClient(_httpClient, _settings, provider.Object);
        }

        [TestMethod]
        public async Task AddTest()
        {
            var personId = DataGenerator.RandomId();
            var medicationId = DataGenerator.RandomId();
            var association = DataGenerator.RandomPersonMedicationAssociation(personId, medicationId, true);
            var json = JsonSerializer.Serialize(association);
            _httpClient.AddResponse(json);

            var added = await _client.AddAsync(personId, medicationId, association.DailyDose, association.Stock, association.LastTaken);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.IsNotNull(added);
            Assert.AreEqual(association.Id, added.Id);
            Assert.AreEqual(personId, added.PersonId);
            Assert.AreEqual(medicationId, added.MedicationId);
            Assert.AreEqual(association.DailyDose, added.DailyDose);
            Assert.AreEqual(association.Stock, added.Stock);
            Assert.AreEqual(association.LastTaken, added.LastTaken);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var personId = DataGenerator.RandomId();
            var medicationId = DataGenerator.RandomId();
            var association = DataGenerator.RandomPersonMedicationAssociation(personId, medicationId, true);
            var json = JsonSerializer.Serialize(association);
            _httpClient.AddResponse(json);

            var updated = await _client.UpdateAsync(association.Id, personId, medicationId, association.DailyDose, association.Stock, association.Active, association.LastTaken);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.IsNotNull(updated);
            Assert.AreEqual(association.Id, updated.Id);
            Assert.AreEqual(personId, updated.PersonId);
            Assert.AreEqual(medicationId, updated.MedicationId);
            Assert.AreEqual(association.DailyDose, updated.DailyDose);
            Assert.AreEqual(association.Stock, updated.Stock);
            Assert.AreEqual(association.LastTaken, updated.LastTaken);
            Assert.AreEqual(association.Active, updated.Active);
        }

        [TestMethod]
        public async Task ActivateTest()
        {
            var personId = DataGenerator.RandomId();
            var medicationId = DataGenerator.RandomId();
            var association = DataGenerator.RandomPersonMedicationAssociation(personId, medicationId, true);
            var json = JsonSerializer.Serialize(association);
            _httpClient.AddResponse(json);

            var updated = await _client.ActivateAsync(association.Id);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual($"{_settings.ApiRoutes[0].Route}/setstate", _httpClient.Requests[0].Uri);

            Assert.IsNotNull(updated);
            Assert.AreEqual(association.Id, updated.Id);
            Assert.AreEqual(personId, updated.PersonId);
            Assert.AreEqual(medicationId, updated.MedicationId);
            Assert.AreEqual(association.DailyDose, updated.DailyDose);
            Assert.AreEqual(association.Stock, updated.Stock);
            Assert.AreEqual(association.LastTaken, updated.LastTaken);
            Assert.AreEqual(association.Active, updated.Active);
        }

        [TestMethod]
        public async Task DeactivateTest()
        {
            var personId = DataGenerator.RandomId();
            var medicationId = DataGenerator.RandomId();
            var association = DataGenerator.RandomPersonMedicationAssociation(personId, medicationId, false);
            var json = JsonSerializer.Serialize(association);
            _httpClient.AddResponse(json);

            var updated = await _client.ActivateAsync(association.Id);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual($"{_settings.ApiRoutes[0].Route}/setstate", _httpClient.Requests[0].Uri);

            Assert.IsNotNull(updated);
            Assert.AreEqual(association.Id, updated.Id);
            Assert.AreEqual(personId, updated.PersonId);
            Assert.AreEqual(medicationId, updated.MedicationId);
            Assert.AreEqual(association.DailyDose, updated.DailyDose);
            Assert.AreEqual(association.Stock, updated.Stock);
            Assert.AreEqual(association.LastTaken, updated.LastTaken);
            Assert.AreEqual(association.Active, updated.Active);
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
            var personId = DataGenerator.RandomId();
            var medicationId = DataGenerator.RandomId();
            var association = DataGenerator.RandomPersonMedicationAssociation(personId, medicationId, true);
            var json = JsonSerializer.Serialize(association);
            _httpClient.AddResponse(json);

            var retrieved = await _client.GetAsync(association.Id);
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{association.Id}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(association.Id, retrieved.Id);
            Assert.AreEqual(association.PersonId, retrieved.PersonId);
            Assert.AreEqual(association.MedicationId, retrieved.MedicationId);
            Assert.AreEqual(association.DailyDose, retrieved.DailyDose);
            Assert.AreEqual(association.Stock, retrieved.Stock);
            Assert.AreEqual(association.LastTaken, retrieved.LastTaken);
        }

        [TestMethod]
        public async Task ListTest()
        {
            var personId = DataGenerator.RandomId();
            var medicationId = DataGenerator.RandomId();
            var association = DataGenerator.RandomPersonMedicationAssociation(personId, medicationId, true);
            var json = JsonSerializer.Serialize(new List<dynamic> { association });
            _httpClient.AddResponse(json);

            var associations = await _client.ListAsync(personId, 1, int.MaxValue);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual($"{_settings.ApiRoutes[0].Route}/{personId}/1/{int.MaxValue}", _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(associations);
            Assert.AreEqual(1, associations.Count);
            Assert.AreEqual(association.Id, associations[0].Id);
            Assert.AreEqual(personId, associations[0].PersonId);
            Assert.AreEqual(medicationId, associations[0].MedicationId);
            Assert.AreEqual(association.DailyDose, associations[0].DailyDose);
            Assert.AreEqual(association.Stock, associations[0].Stock);
            Assert.AreEqual(association.LastTaken, associations[0].LastTaken);
        }
    }
}

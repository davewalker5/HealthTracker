using System.Text.Json;
using HealthTracker.Client.ApiClient;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.Entities.Food;
using HealthTracker.Tests.Mocks;
using Microsoft.Extensions.Logging;
using Moq;

namespace HealthTracker.Tests.NutritionalValues
{
    [TestClass]
    public class NutritionalValueClientTest
    {
        private readonly string ApiToken = DataGenerator.RandomApiToken();
        private readonly MockHealthTrackerHttpClient _httpClient = new();
        private INutritionalValueClient _client;

        private readonly HealthTrackerApplicationSettings _settings = new()
        {
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "NutritionalValue", Route = "/nutritionalvalue" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(ApiToken);
            var logger = new Mock<ILogger<NutritionalValueClient>>();
            _client = new NutritionalValueClient(_httpClient, _settings, provider.Object, logger.Object);
        }

        [TestMethod]
        public async Task AddTest()
        {
            var nutritionalValue = DataGenerator.RandomNutritionalValue();
            var json = JsonSerializer.Serialize(new
            {
                Calories = nutritionalValue.Calories,
                Fat = nutritionalValue.Fat,
                SaturatedFat = nutritionalValue.SaturatedFat,
                Protein = nutritionalValue.Protein,
                Carbohydrates = nutritionalValue.Carbohydrates,
                Sugar = nutritionalValue.Sugar,
                Fibre = nutritionalValue.Fibre
            });
            _httpClient.AddResponse(json);

            var added = await _client.AddAsync(
                nutritionalValue.Calories,
                nutritionalValue.Fat,
                nutritionalValue.SaturatedFat,
                nutritionalValue.Protein,
                nutritionalValue.Carbohydrates,
                nutritionalValue.Sugar,
                nutritionalValue.Fibre);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.AreEqual(json, await _httpClient.Requests[0].Content.ReadAsStringAsync());
            Assert.IsNotNull(added);
            Assert.AreEqual(nutritionalValue.Calories, added.Calories);
            Assert.AreEqual(nutritionalValue.Fat, added.Fat);
            Assert.AreEqual(nutritionalValue.SaturatedFat, added.SaturatedFat);
            Assert.AreEqual(nutritionalValue.Protein, added.Protein);
            Assert.AreEqual(nutritionalValue.Carbohydrates, added.Carbohydrates);
            Assert.AreEqual(nutritionalValue.Sugar, added.Sugar);
            Assert.AreEqual(nutritionalValue.Fibre, added.Fibre);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var nutritionalValue = DataGenerator.RandomNutritionalValue();
            var json = JsonSerializer.Serialize(nutritionalValue);
            _httpClient.AddResponse(json);

            var updated = await _client.UpdateAsync(
                nutritionalValue.Id,
                nutritionalValue.Calories,
                nutritionalValue.Fat,
                nutritionalValue.SaturatedFat,
                nutritionalValue.Protein,
                nutritionalValue.Carbohydrates,
                nutritionalValue.Sugar,
                nutritionalValue.Fibre);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.AreEqual(json, await _httpClient.Requests[0].Content.ReadAsStringAsync());
            Assert.IsNotNull(updated);
            Assert.AreEqual(nutritionalValue.Id, updated.Id);
            Assert.AreEqual(nutritionalValue.Calories, updated.Calories);
            Assert.AreEqual(nutritionalValue.Fat, updated.Fat);
            Assert.AreEqual(nutritionalValue.SaturatedFat, updated.SaturatedFat);
            Assert.AreEqual(nutritionalValue.Protein, updated.Protein);
            Assert.AreEqual(nutritionalValue.Carbohydrates, updated.Carbohydrates);
            Assert.AreEqual(nutritionalValue.Sugar, updated.Sugar);
            Assert.AreEqual(nutritionalValue.Fibre, updated.Fibre);
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
    }
}

using System.Text.Json;
using HealthTracker.Client.ApiClient;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.Entities.Food;
using HealthTracker.Tests.Mocks;
using Microsoft.Extensions.Logging;
using Moq;

namespace HealthTracker.Tests.Beverages
{
    [TestClass]
    public class BeverageClientTest
    {
        private readonly string ApiToken = DataGenerator.RandomApiToken();
        private readonly MockHealthTrackerHttpClient _httpClient = new();
        private IBeverageClient _client;

        private readonly HealthTrackerApplicationSettings _settings = new()
        {
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "Beverage", Route = "/beverage" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(ApiToken);
            var logger = new Mock<ILogger<BeverageClient>>();
            _client = new BeverageClient(_httpClient, _settings, provider.Object, logger.Object);
        }

        [TestMethod]
        public async Task AddTest()
        {
            var beverage = DataGenerator.RandomBeverage();
            var json = JsonSerializer.Serialize(new { beverage.Name, beverage.TypicalABV, beverage.IsHydrating, beverage.IsAlcohol });
            _httpClient.AddResponse(json);

            var added = await _client.AddAsync(beverage.Name, beverage.TypicalABV, beverage.IsHydrating, beverage.IsAlcohol);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.AreEqual(json, await _httpClient.Requests[0].Content.ReadAsStringAsync());
            Assert.IsNotNull(added);
            Assert.AreEqual(beverage.Name, added.Name);
            Assert.AreEqual(beverage.TypicalABV, added.TypicalABV);
            Assert.AreEqual(beverage.IsHydrating, added.IsHydrating);
            Assert.AreEqual(beverage.IsAlcohol, added.IsAlcohol);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var beverage = DataGenerator.RandomBeverage();
            var json = JsonSerializer.Serialize(beverage);
            _httpClient.AddResponse(json);

            var updated = await _client.UpdateAsync(beverage.Id, beverage.Name, beverage.TypicalABV, beverage.IsHydrating, beverage.IsAlcohol);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.AreEqual(json, await _httpClient.Requests[0].Content.ReadAsStringAsync());
            Assert.IsNotNull(updated);
            Assert.AreEqual(beverage.Id, updated.Id);
            Assert.AreEqual(beverage.Name, updated.Name);
            Assert.AreEqual(beverage.TypicalABV, updated.TypicalABV);
            Assert.AreEqual(beverage.IsHydrating, updated.IsHydrating);
            Assert.AreEqual(beverage.IsAlcohol, updated.IsAlcohol);
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
        public async Task ListTest()
        {
            var beverage = DataGenerator.RandomBeverage();
            var json = JsonSerializer.Serialize<List<Beverage>>([beverage]);
            _httpClient.AddResponse(json);

            var beverages = await _client.ListAsync(1, int.MaxValue);
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(beverages);
            Assert.AreEqual(1, beverages.Count);
            Assert.AreEqual(beverage.Id, beverages[0].Id);
            Assert.AreEqual(beverage.Name, beverages[0].Name);
            Assert.AreEqual(beverage.TypicalABV, beverages[0].TypicalABV);
            Assert.AreEqual(beverage.IsHydrating, beverages[0].IsHydrating);
            Assert.AreEqual(beverage.IsAlcohol, beverages[0].IsAlcohol);
        }
    }
}

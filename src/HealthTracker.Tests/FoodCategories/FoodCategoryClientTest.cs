using System.Text.Json;
using HealthTracker.Client.ApiClient;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.Entities.Food;
using HealthTracker.Tests.Mocks;
using Microsoft.Extensions.Logging;
using Moq;

namespace HealthTracker.Tests.FoodCategories
{
    [TestClass]
    public class FoodCategoryClientTest
    {
        private readonly string ApiToken = DataGenerator.RandomApiToken();
        private readonly MockHealthTrackerHttpClient _httpClient = new();
        private IFoodCategoryClient _client;

        private readonly HealthTrackerApplicationSettings _settings = new()
        {
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "FoodCategory", Route = "/foodcategory" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(ApiToken);
            var logger = new Mock<ILogger<FoodCategoryClient>>();
            _client = new FoodCategoryClient(_httpClient, _settings, provider.Object, logger.Object);
        }

        [TestMethod]
        public async Task AddTest()
        {
            var foodCategory = DataGenerator.RandomFoodCategory();
            var json = JsonSerializer.Serialize(new { foodCategory.Name });
            _httpClient.AddResponse(json);

            var added = await _client.AddAsync(foodCategory.Name);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.AreEqual(json, await _httpClient.Requests[0].Content.ReadAsStringAsync());
            Assert.IsNotNull(added);
            Assert.AreEqual(foodCategory.Name, added.Name);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var foodCategory = DataGenerator.RandomFoodCategory();
            var json = JsonSerializer.Serialize(foodCategory);
            _httpClient.AddResponse(json);

            var updated = await _client.UpdateAsync(foodCategory.Id, foodCategory.Name);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.AreEqual(json, await _httpClient.Requests[0].Content.ReadAsStringAsync());
            Assert.IsNotNull(updated);
            Assert.AreEqual(foodCategory.Id, updated.Id);
            Assert.AreEqual(foodCategory.Name, updated.Name);
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
            var foodCategory = DataGenerator.RandomFoodCategory();
            var json = JsonSerializer.Serialize<List<FoodCategory>>([foodCategory]);
            _httpClient.AddResponse(json);

            var categories = await _client.ListAsync(1, int.MaxValue);
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(categories);
            Assert.AreEqual(1, categories.Count);
            Assert.AreEqual(foodCategory.Id, categories[0].Id);
            Assert.AreEqual(foodCategory.Name, categories[0].Name);
        }
    }
}

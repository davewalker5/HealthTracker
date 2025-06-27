using System.Text.Json;
using HealthTracker.Client.ApiClient;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.Tests.Mocks;
using Microsoft.Extensions.Logging;
using Moq;

namespace HealthTracker.Tests.FoodItems
{
    [TestClass]
    public class FoodItemClientTest
    {
        private readonly string ApiToken = DataGenerator.RandomApiToken();
        private MockHealthTrackerHttpClient _httpClient = new();
        private IFoodItemClient _client;
        private string _filePath;

        private readonly HealthTrackerApplicationSettings _settings = new()
        {
            DefaultTimePeriodDays = 7,
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "FoodItem", Route = "/fooditem" },
                new() { Name = "ExportFoodItem", Route = "/export/fooditem" },
                new() { Name = "ImportFoodItem", Route = "/import/fooditem" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(ApiToken);
            var logger = new Mock<ILogger<FoodItemClient>>();
            _client = new FoodItemClient(_httpClient, _settings, provider.Object, logger.Object);
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
            var item = DataGenerator.RandomFoodItem();
            var json = JsonSerializer.Serialize(item);
            _httpClient.AddResponse(json);

            var added = await _client.AddAsync(
                item.Name,
                item.Portion,
                item.FoodCategoryId,
                item.NutritionalValueId);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.IsNotNull(added);
            Assert.AreEqual(item.Id, added.Id);
            Assert.AreEqual(item.Name, added.Name);
            Assert.AreEqual(item.Portion, added.Portion);
            Assert.AreEqual(item.FoodCategoryId, added.FoodCategoryId);
            Assert.AreEqual(item.NutritionalValueId, added.NutritionalValueId);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var item = DataGenerator.RandomFoodItem();
            var json = JsonSerializer.Serialize(item);
            _httpClient.AddResponse(json);

            var updated = await _client.UpdateAsync(
                item.Id,
                item.Name,
                item.Portion,
                item.FoodCategoryId,
                item.NutritionalValueId);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.IsNotNull(updated);
            Assert.AreEqual(item.Id, updated.Id);
            Assert.AreEqual(item.Name, updated.Name);
            Assert.AreEqual(item.Portion, updated.Portion);
            Assert.AreEqual(item.FoodCategoryId, updated.FoodCategoryId);
            Assert.AreEqual(item.NutritionalValueId, updated.NutritionalValueId);
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
            var item = DataGenerator.RandomFoodItem();
            var json = JsonSerializer.Serialize(item);
            _httpClient.AddResponse(json);

            var retrieved = await _client.GetAsync(item.Id);
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{item.Id}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(item.Id, retrieved.Id);
            Assert.AreEqual(item.Name, retrieved.Name);
            Assert.AreEqual(item.Portion, retrieved.Portion);
            Assert.AreEqual(item.FoodCategoryId, retrieved.FoodCategoryId);
            Assert.AreEqual(item.NutritionalValueId, retrieved.NutritionalValueId);
        }

        [TestMethod]
        public async Task ListTest()
        {
            var item = DataGenerator.RandomFoodItem();
            var json = JsonSerializer.Serialize(new List<dynamic> { item });
            _httpClient.AddResponse(json);

            var items = await _client.ListAsync(item.FoodCategoryId,  1, int.MaxValue);

            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{item.FoodCategoryId}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(items);
            Assert.AreEqual(1, items.Count);
            Assert.AreEqual(item.Id, items[0].Id);
            Assert.AreEqual(item.Name, items[0].Name);
            Assert.AreEqual(item.Portion, items[0].Portion);
            Assert.AreEqual(item.FoodCategoryId, items[0].FoodCategoryId);
            Assert.AreEqual(item.NutritionalValueId, items[0].NutritionalValueId);
        }

        [TestMethod]
        public async Task ImportTest()
        {
            _httpClient.AddResponse("");

            var item = DataGenerator.RandomFoodItem();
            var record = $@"""{item.Name}"",""{item.FoodCategory.Name}"",""{item.Portion}"",""{item.NutritionalValue.Calories}"",""{item.NutritionalValue.Fat}"",""{item.NutritionalValue.SaturatedFat}"",""{item.NutritionalValue.Protein}"",""{item.NutritionalValue.Carbohydrates}"",""{item.NutritionalValue.Sugar}"",""{item.NutritionalValue.Fibre}""";

            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _client.ImportFromFileAsync(_filePath);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "ImportFoodItem").Route, _httpClient.Requests[0].Uri);
        }

        [TestMethod]
        public async Task ExportTest()
        {
            _httpClient.AddResponse("");
            _filePath = DataGenerator.TemporaryCsvFilePath();

            await _client.ExportAsync(_filePath);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "ExportFoodItem").Route, _httpClient.Requests[0].Uri);
        }
    }
}
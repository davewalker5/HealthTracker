using System.Text.Json;
using System.Web;
using HealthTracker.Client.ApiClient;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.Tests.Mocks;
using Microsoft.Extensions.Logging;
using Moq;

namespace HealthTracker.Tests.MealFoodItems
{
    [TestClass]
    public class MealFoodItemMeasurementClientTest
    {
        private readonly string ApiToken = DataGenerator.RandomApiToken();
        private MockHealthTrackerHttpClient _httpClient = new();
        private IMealFoodItemClient _client;
        private string _filePath;

        private readonly HealthTrackerApplicationSettings _settings = new()
        {
            DefaultTimePeriodDays = 7,
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "MealFoodItem", Route = "/mealfooditem" },
                new() { Name = "ExportMealFoodItem", Route = "/export/mealfooditem" },
                new() { Name = "ImportMealFoodItem", Route = "/import/mealfooditem" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(ApiToken);
            var logger = new Mock<ILogger<MealFoodItemClient>>();
            _client = new MealFoodItemClient(_httpClient, _settings, provider.Object, logger.Object);
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
            var relationship = DataGenerator.RandomMealFoodItemRelationship();
            var json = JsonSerializer.Serialize(relationship);
            _httpClient.AddResponse(json);

            var added = await _client.AddAsync(relationship.MealId, relationship.FoodItemId, relationship.Quantity);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.IsNotNull(added);
            Assert.AreEqual(relationship.Id, added.Id);
            Assert.AreEqual(relationship.MealId, added.MealId);
            Assert.AreEqual(relationship.FoodItemId, added.FoodItemId);
            Assert.AreEqual(relationship.Quantity, added.Quantity);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var relationship = DataGenerator.RandomMealFoodItemRelationship();
            var json = JsonSerializer.Serialize(relationship);
            _httpClient.AddResponse(json);

            var updated = await _client.UpdateAsync(relationship.Id, relationship.MealId, relationship.FoodItemId, relationship.Quantity);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.IsNotNull(updated);
            Assert.AreEqual(relationship.Id, updated.Id);
            Assert.AreEqual(relationship.MealId, updated.MealId);
            Assert.AreEqual(relationship.FoodItemId, updated.FoodItemId);
            Assert.AreEqual(relationship.Quantity, updated.Quantity);
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
            var measurement = DataGenerator.RandomMealFoodItemRelationship();
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
        }

        [TestMethod]
        public async Task ListTest()
        {
            var relationship = DataGenerator.RandomMealFoodItemRelationship();
            var json = JsonSerializer.Serialize(new List<dynamic> { relationship });
            _httpClient.AddResponse(json);

            var retrieved = await _client.ListAsync(relationship.MealId);
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/list/{relationship.MealId}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(1, retrieved.Count);
            Assert.AreEqual(relationship.Id, retrieved.First().Id);
            Assert.AreEqual(relationship.MealId, retrieved.First().MealId);
            Assert.AreEqual(relationship.FoodItemId, retrieved.First().FoodItemId);
            Assert.AreEqual(relationship.Quantity, retrieved.First().Quantity);
        }

        [TestMethod]
        public async Task ImportTest()
        {
            _httpClient.AddResponse("");

            var meal = DataGenerator.RandomTitleCasePhrase(5, 3, 15);
            var relationship = DataGenerator.RandomMealFoodItemRelationship();
            var record = $@"""{meal}"",""{relationship.FoodItem.Name}"",""{relationship.Quantity}"",""{relationship.NutritionalValue.Calories}"",""{relationship.NutritionalValue.Fat}"",""{relationship.NutritionalValue.SaturatedFat}"",""{relationship.NutritionalValue.Protein}"",""{relationship.NutritionalValue.Carbohydrates}"",""{relationship.NutritionalValue.Sugar}"",""{relationship.NutritionalValue.Fibre}""";

            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _client.ImportFromFileAsync(_filePath);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "ImportMealFoodItem").Route, _httpClient.Requests[0].Uri);
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
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "ExportMealFoodItem").Route, _httpClient.Requests[0].Uri);
        }
    }
}
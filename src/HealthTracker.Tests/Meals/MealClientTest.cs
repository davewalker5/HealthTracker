using System.Text.Json;
using HealthTracker.Client.ApiClient;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.Tests.Mocks;
using Microsoft.Extensions.Logging;
using Moq;

namespace HealthTracker.Tests.Meals
{
    [TestClass]
    public class MealClientTest
    {
        private readonly string ApiToken = DataGenerator.RandomApiToken();
        private MockHealthTrackerHttpClient _httpClient = new();
        private IMealClient _client;
        private string _filePath;

        private readonly HealthTrackerApplicationSettings _settings = new()
        {
            DefaultTimePeriodDays = 7,
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "Meal", Route = "/meal" },
                new() { Name = "ExportMeal", Route = "/export/meal" },
                new() { Name = "ImportMeal", Route = "/import/meal" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(ApiToken);
            var logger = new Mock<ILogger<MealClient>>();
            _client = new MealClient(_httpClient, _settings, provider.Object, logger.Object);
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
            var meal = DataGenerator.RandomMeal();
            var json = JsonSerializer.Serialize(meal);
            _httpClient.AddResponse(json);

            var added = await _client.AddAsync(
                meal.Name,
                meal.Portions,
                meal.FoodSource.Id,
                meal.NutritionalValueId);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.IsNotNull(added);
            Assert.AreEqual(meal.Id, added.Id);
            Assert.AreEqual(meal.Name, added.Name);
            Assert.AreEqual(meal.Portions, added.Portions);
            Assert.AreEqual(meal.FoodSourceId, added.FoodSourceId);
            Assert.AreEqual(meal.NutritionalValueId, added.NutritionalValueId);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var meal = DataGenerator.RandomMeal();
            var json = JsonSerializer.Serialize(meal);
            _httpClient.AddResponse(json);

            var updated = await _client.UpdateAsync(
                meal.Id,
                meal.Name,
                meal.Portions,
                meal.FoodSource.Id,
                meal.NutritionalValueId);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.IsNotNull(updated);
            Assert.AreEqual(meal.Id, updated.Id);
            Assert.AreEqual(meal.Name, updated.Name);
            Assert.AreEqual(meal.Portions, updated.Portions);
            Assert.AreEqual(meal.FoodSourceId, updated.FoodSourceId);
            Assert.AreEqual(meal.NutritionalValueId, updated.NutritionalValueId);
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
            var meal = DataGenerator.RandomMeal();
            var json = JsonSerializer.Serialize(meal);
            _httpClient.AddResponse(json);

            var retrieved = await _client.GetAsync(meal.Id);
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{meal.Id}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(meal.Id, retrieved.Id);
            Assert.AreEqual(meal.Name, retrieved.Name);
            Assert.AreEqual(meal.Portions, retrieved.Portions);
            Assert.AreEqual(meal.FoodSourceId, retrieved.FoodSourceId);
            Assert.AreEqual(meal.NutritionalValueId, retrieved.NutritionalValueId);
        }

        [TestMethod]
        public async Task ListTest()
        {
            var meal = DataGenerator.RandomMeal();
            var json = JsonSerializer.Serialize(new List<dynamic> { meal });
            _httpClient.AddResponse(json);

            var meals = await _client.ListAsync(meal.FoodSource.Id, 1, int.MaxValue);

            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{meal.FoodSource.Id}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(meals);
            Assert.AreEqual(1, meals.Count);
            Assert.AreEqual(meal.Id, meals[0].Id);
            Assert.AreEqual(meal.Name, meals[0].Name);
            Assert.AreEqual(meal.Portions, meals[0].Portions);
            Assert.AreEqual(meal.FoodSourceId, meals[0].FoodSourceId);
            Assert.AreEqual(meal.NutritionalValueId, meals[0].NutritionalValueId);
        }

        [TestMethod]
        public async Task ImportTest()
        {
            _httpClient.AddResponse("");

            var meal = DataGenerator.RandomMeal();
            var record = $@"""{meal.Name}"",""{meal.Portions}"",""{meal.NutritionalValue.Calories}"",""{meal.NutritionalValue.Fat}"",""{meal.NutritionalValue.SaturatedFat}"",""{meal.NutritionalValue.Protein}"",""{meal.NutritionalValue.Carbohydrates}"",""{meal.NutritionalValue.Sugar}"",""{meal.NutritionalValue.Fibre}""";

            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _client.ImportFromFileAsync(_filePath);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "ImportMeal").Route, _httpClient.Requests[0].Uri);
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
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "ExportMeal").Route, _httpClient.Requests[0].Uri);
        }
    }
}
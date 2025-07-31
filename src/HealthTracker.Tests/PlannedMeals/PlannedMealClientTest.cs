using System.Text.Json;
using System.Web;
using HealthTracker.Client.ApiClient;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.Tests.Mocks;
using Microsoft.Extensions.Logging;
using Moq;

namespace HealthTracker.Tests.PlannedMeals
{
    [TestClass]
    public class PlannedMealClientTest
    {
        private readonly string ApiToken = DataGenerator.RandomApiToken();
        private MockHealthTrackerHttpClient _httpClient = new();
        private IPlannedMealClient _client;
        private string _filePath;

        private readonly HealthTrackerApplicationSettings _settings = new()
        {
            DefaultTimePeriodDays = 7,
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "PlannedMeal", Route = "/plannedmeal" },
                new() { Name = "ExportPlannedMeal", Route = "/export/plannedmeal" },
                new() { Name = "ImportPlannedMeal", Route = "/import/plannedmeal" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(ApiToken);
            var logger = new Mock<ILogger<PlannedMealClient>>();
            _client = new PlannedMealClient(_httpClient, _settings, provider.Object, logger.Object);
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
            var person = DataGenerator.RandomPerson(10, 90);
            var plannedMeal = DataGenerator.RandomPlannedMeal();
            plannedMeal.PersonId = person.Id;

            var json = JsonSerializer.Serialize(plannedMeal);
            _httpClient.AddResponse(json);

            var added = await _client.AddAsync(person.Id, plannedMeal.MealType, plannedMeal.Date, plannedMeal.MealId);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.IsNotNull(added);
            Assert.AreEqual(plannedMeal.PersonId, added.PersonId);
            Assert.AreEqual(plannedMeal.Date, added.Date);
            Assert.AreEqual(plannedMeal.MealType, added.MealType);
            Assert.AreEqual(plannedMeal.MealId, added.MealId);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var person = DataGenerator.RandomPerson(10, 90);
            var plannedMeal = DataGenerator.RandomPlannedMeal();
            plannedMeal.PersonId = person.Id;

            var json = JsonSerializer.Serialize(plannedMeal);
            _httpClient.AddResponse(json);

            var updated = await _client.UpdateAsync(plannedMeal.Id, person.Id, plannedMeal.MealType, plannedMeal.Date, plannedMeal.MealId);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.IsNotNull(updated);
            Assert.AreEqual(plannedMeal.Id, updated.Id);
            Assert.AreEqual(plannedMeal.PersonId, updated.PersonId);
            Assert.AreEqual(plannedMeal.Date, updated.Date);
            Assert.AreEqual(plannedMeal.MealType, updated.MealType);
            Assert.AreEqual(plannedMeal.MealId, updated.MealId);
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
        public async Task PurgeTest()
        {
            var date = DataGenerator.RandomDateInYear(2025);
            var personId = DataGenerator.RandomId();
            _httpClient.AddResponse("");
            await _client.PurgeAsync(personId, date);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual($"{_settings.ApiRoutes[0].Route}/purge", _httpClient.Requests[0].Uri);
        }

        [TestMethod]
        public async Task GetTest()
        {
            var person = DataGenerator.RandomPerson(10, 90);
            var plannedMeal = DataGenerator.RandomPlannedMeal();
            plannedMeal.PersonId = person.Id;

            var json = JsonSerializer.Serialize(plannedMeal);
            _httpClient.AddResponse(json);

            var retrieved = await _client.GetAsync(plannedMeal.Id);
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{plannedMeal.Id}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(plannedMeal.Id, retrieved.Id);
            Assert.AreEqual(plannedMeal.PersonId, retrieved.PersonId);
            Assert.AreEqual(plannedMeal.Date, retrieved.Date);
            Assert.AreEqual(plannedMeal.MealType, retrieved.MealType);
            Assert.AreEqual(plannedMeal.MealId, retrieved.MealId);
        }

        [TestMethod]
        public async Task ListTest()
        {
            var person = DataGenerator.RandomPerson(10, 90);
            var plannedMeal = DataGenerator.RandomPlannedMeal();
            plannedMeal.PersonId = person.Id;

            var json = JsonSerializer.Serialize(new List<dynamic> { plannedMeal });
            _httpClient.AddResponse(json);

            var to = plannedMeal.Date.AddDays(1);
            var from = plannedMeal.Date.AddDays(-1);
            var encodedFrom = HttpUtility.UrlEncode(from.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(to.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{person.Id}/{encodedFrom}/{encodedTo}/1/{int.MaxValue}";

            var retrieved = await _client.ListAsync(person.Id, from, to, 1, int.MaxValue);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(1, retrieved.Count);
            Assert.AreEqual(plannedMeal.Id, retrieved[0].Id);
            Assert.AreEqual(plannedMeal.PersonId, retrieved[0].PersonId);
            Assert.AreEqual(plannedMeal.MealType, retrieved[0].MealType);
            Assert.AreEqual(plannedMeal.Date, retrieved[0].Date);
            Assert.AreEqual(plannedMeal.MealId, retrieved[0].MealId);
        }

        [TestMethod]
        public async Task ImportTest()
        {
            var person = DataGenerator.RandomPerson(10, 90);
            var plannedMeal = DataGenerator.RandomPlannedMeal();
            plannedMeal.PersonId = person.Id;

            _httpClient.AddResponse("");

            var record = $@"""{person.Id}"",""{person.Name}"",""{plannedMeal.Date:dd-MMM-yyyy HH:mm:ss}"",""{plannedMeal.MealType}"",""{plannedMeal.Meal}"",""{plannedMeal.Meal.FoodSource.Name}"",""{plannedMeal.Meal.Reference}""";

            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _client.ImportFromFileAsync(_filePath);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "ImportPlannedMeal").Route, _httpClient.Requests[0].Uri);
        }

        [TestMethod]
        public async Task ExportTest()
        {
            var personId = DataGenerator.RandomId();
            var to = DataGenerator.RandomDateInYear(2025);
            var from = to.AddDays(-30);

            _httpClient.AddResponse("");
            _filePath = DataGenerator.TemporaryCsvFilePath();

            await _client.ExportAsync(personId, from, to, _filePath);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes.First(x => x.Name == "ExportPlannedMeal").Route, _httpClient.Requests[0].Uri);
        }

        [TestMethod]
        public async Task GetShoppingListTest()
        {
            var personId = DataGenerator.RandomId();
            var to = DataGenerator.RandomDateInYear(2025);
            var from = to.AddDays(-30);
            var item = DataGenerator.RandomShoppingListItem();

            var json = JsonSerializer.Serialize(new List<dynamic> { item });
            _httpClient.AddResponse(json);

            var encodedFrom = HttpUtility.UrlEncode(from.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(to.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{personId}/{encodedFrom}/{encodedTo}";

            var retrieved = await _client.GetShoppingListAsync(personId, from, to);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(1, retrieved.Count);
            Assert.AreEqual(item.FoodItemId, retrieved.First().FoodItemId);
            Assert.AreEqual(item.Portion, retrieved.First().Portion);
            Assert.AreEqual(item.Quantity, retrieved.First().Quantity);
            Assert.AreEqual(item.Item, retrieved.First().Item);
        }
    }
}
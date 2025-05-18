using System.Text.Json;
using HealthTracker.Client.ApiClient;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.Entities.Measurements;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.ActivityTypes
{
    [TestClass]
    public class ActivityTypeClientTest
    {
        private readonly string ApiToken = DataGenerator.RandomApiToken();
        private readonly MockHealthTrackerHttpClient _httpClient = new();
        private IActivityTypeClient _client;

        private readonly HealthTrackerApplicationSettings _settings = new()
        {
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "ActivityType", Route = "/activitytype" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(ApiToken);
            _client = new ActivityTypeClient(_httpClient, _settings, provider.Object);
        }

        [TestMethod]
        public async Task AddTest()
        {
            var activityType = DataGenerator.RandomActivityType();
            var json = JsonSerializer.Serialize(new { activityType.Description, activityType.DistanceBased });
            _httpClient.AddResponse(json);

            var added = await _client.AddActivityTypeAsync(activityType.Description, activityType.DistanceBased);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.AreEqual(json, await _httpClient.Requests[0].Content.ReadAsStringAsync());
            Assert.IsNotNull(added);
            Assert.AreEqual(activityType.Description, added.Description);
            Assert.AreEqual(activityType.DistanceBased, added.DistanceBased);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var activityType = DataGenerator.RandomActivityType();
            var json = JsonSerializer.Serialize(activityType);
            _httpClient.AddResponse(json);

            var updated = await _client.UpdateActivityTypeAsync(activityType.Id, activityType.Description, true);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Put, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.AreEqual(json, await _httpClient.Requests[0].Content.ReadAsStringAsync());
            Assert.IsNotNull(updated);
            Assert.AreEqual(activityType.Id, updated.Id);
            Assert.AreEqual(activityType.Description, updated.Description);
            Assert.AreEqual(activityType.DistanceBased, updated.DistanceBased);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            var id = DataGenerator.RandomId();
            await _client.DeleteActivityTypeAsync(id);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Delete, _httpClient.Requests[0].Method);
            Assert.AreEqual($"{_settings.ApiRoutes[0].Route}/{id}", _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
        }

        [TestMethod]
        public async Task ListTest()
        {
            var activityType = DataGenerator.RandomActivityType();
            var json = JsonSerializer.Serialize<List<ActivityType>>([activityType]);
            _httpClient.AddResponse(json);

            var activities = await _client.ListActivityTypesAsync(1, int.MaxValue);
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.AreEqual(expectedRoute, _httpClient.Requests[0].Uri);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(activities);
            Assert.AreEqual(1, activities.Count);
            Assert.AreEqual(activityType.Id, activities[0].Id);
            Assert.AreEqual(activityType.Description, activities[0].Description);
            Assert.AreEqual(activityType.DistanceBased, activities[0].DistanceBased);
        }
    }
}

using System.Text.Json;
using System.Web;
using HealthTracker.Client.ApiClient;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.Tests.Mocks;
using Microsoft.Extensions.Logging;
using Moq;

namespace HealthTracker.Tests.JobStatuses
{
    [TestClass]
    public class JobStatusClientTest
    {
        private readonly string _apiToken = DataGenerator.RandomApiToken();
        private readonly MockHealthTrackerHttpClient _httpClient = new();
        private IJobStatusClient _client;

        private readonly HealthTrackerApplicationSettings _settings = new()
        {
            DefaultTimePeriodDays = 7,
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "JobStatuses", Route = "/jobstatus" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(_apiToken);
            var logger = new Mock<ILogger<JobStatusClient>>();
            _client = new JobStatusClient(_httpClient, _settings, provider.Object, logger.Object);
        }

        [TestMethod]
        public async Task ListWithNoDateRangeTest()
        {
            var status = DataGenerator.RandomJobStatus();
            var json = JsonSerializer.Serialize(new List<dynamic>() { status });
            _httpClient.AddResponse(json);

            var retrieved = await _client.ListAsync(null, null, 1, int.MaxValue);

            var expectedTo = DateTime.Now;
            var expectedFrom = expectedTo.AddDays(-_settings.DefaultTimePeriodDays);
            var encodedFrom = HttpUtility.UrlEncode(expectedFrom.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(expectedTo.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{encodedFrom}/{encodedTo}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {_apiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            RouteChecker.ConfirmDateBasedRoutesAreEqual(expectedRoute, _httpClient.Requests[0].Uri, [1, 2]);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(1, retrieved.Count);
            Assert.AreEqual(status.Id, retrieved[0].Id);
            Assert.AreEqual(status.Name, retrieved[0].Name);
            Assert.AreEqual(status.Parameters, retrieved[0].Parameters);
            Assert.AreEqual(status.Start, retrieved[0].Start);
            Assert.AreEqual(status.End, retrieved[0].End);
            Assert.AreEqual(status.Error, retrieved[0].Error);
        }

        [TestMethod]
        public async Task ListWithFromDateOnlyTest()
        {
            var status = DataGenerator.RandomJobStatus();
            var json = JsonSerializer.Serialize(new List<dynamic>() { status });
            _httpClient.AddResponse(json);

            var from = status.Start.AddDays(-DataGenerator.RandomInt(30, 90));
            var retrieved = await _client.ListAsync(from, null, 1, int.MaxValue);

            var expectedTo = DateTime.Now;
            var encodedFrom = HttpUtility.UrlEncode(from.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(expectedTo.ToString("yyyy-MM-dd H:mm:ss"));

            Assert.AreEqual($"Bearer {_apiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            Assert.IsTrue(_httpClient.Requests[0].Uri.StartsWith(_settings.ApiRoutes[0].Route));

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(1, retrieved.Count);
            Assert.AreEqual(status.Id, retrieved[0].Id);
            Assert.AreEqual(status.Name, retrieved[0].Name);
            Assert.AreEqual(status.Parameters, retrieved[0].Parameters);
            Assert.AreEqual(status.Start, retrieved[0].Start);
            Assert.AreEqual(status.End, retrieved[0].End);
            Assert.AreEqual(status.Error, retrieved[0].Error);
        }

        [TestMethod]
        public async Task ListWithEndDateOnlyTest()
        {
            var status = DataGenerator.RandomJobStatus();
            var json = JsonSerializer.Serialize(new List<dynamic>() { status });
            _httpClient.AddResponse(json);

            var to = status.Start.AddDays(DataGenerator.RandomInt(30, 90));
            var retrieved = await _client.ListAsync(null, to, 1, int.MaxValue);

            var expectedFrom = to.AddDays(-_settings.DefaultTimePeriodDays);
            var encodedFrom = HttpUtility.UrlEncode(expectedFrom.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(to.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{encodedFrom}/{encodedTo}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {_apiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            RouteChecker.ConfirmDateBasedRoutesAreEqual(expectedRoute, _httpClient.Requests[0].Uri, [1, 2]);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(1, retrieved.Count);
            Assert.AreEqual(status.Id, retrieved[0].Id);
            Assert.AreEqual(status.Name, retrieved[0].Name);
            Assert.AreEqual(status.Parameters, retrieved[0].Parameters);
            Assert.AreEqual(status.Start, retrieved[0].Start);
            Assert.AreEqual(status.End, retrieved[0].End);
            Assert.AreEqual(status.Error, retrieved[0].Error);
        }

        [TestMethod]
        public async Task ListWithFromAndToDatesTest()
        {
            var status = DataGenerator.RandomJobStatus();
            var json = JsonSerializer.Serialize(new List<dynamic>() { status });
            _httpClient.AddResponse(json);

            var from = status.Start.AddDays(-DataGenerator.RandomInt(30, 90));
            var to = from.AddDays(DataGenerator.RandomInt(30, 90));
            var retrieved = await _client.ListAsync(from, to, 1, int.MaxValue);

            var encodedFrom = HttpUtility.UrlEncode(from.ToString("yyyy-MM-dd H:mm:ss"));
            var encodedTo = HttpUtility.UrlEncode(to.ToString("yyyy-MM-dd H:mm:ss"));
            var expectedRoute = $"{_settings.ApiRoutes[0].Route}/{encodedFrom}/{encodedTo}/1/{int.MaxValue}";

            Assert.AreEqual($"Bearer {_apiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Get, _httpClient.Requests[0].Method);
            RouteChecker.ConfirmDateBasedRoutesAreEqual(expectedRoute, _httpClient.Requests[0].Uri, [1, 2]);

            Assert.IsNull(_httpClient.Requests[0].Content);
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(1, retrieved.Count);
            Assert.AreEqual(status.Id, retrieved[0].Id);
            Assert.AreEqual(status.Name, retrieved[0].Name);
            Assert.AreEqual(status.Parameters, retrieved[0].Parameters);
            Assert.AreEqual(status.Start, retrieved[0].Start);
            Assert.AreEqual(status.End, retrieved[0].End);
            Assert.AreEqual(status.Error, retrieved[0].Error);
        }
    }
}

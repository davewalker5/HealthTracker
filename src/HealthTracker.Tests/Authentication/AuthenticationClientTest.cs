using HealthTracker.Client.ApiClient;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests
{
    [TestClass]
    public class AuthenticationClientTest
    {
        private readonly string ApiToken = DataGenerator.RandomApiToken();
        private MockHealthTrackerHttpClient _httpClient = new();
        private IAuthenticationClient _client;
        private IAuthenticationTokenProvider _provider;
        private string _mockApiToken = null;

        private readonly HealthTrackerApplicationSettings _settings = new()
        {
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "Authenticate", Route = "/users/authenticate" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(m => m.SetToken(It.IsAny<string>())).Callback<string>(val => _mockApiToken = val);
            provider.Setup(m => m.ClearToken()).Callback(() => _mockApiToken = "");
            provider.Setup(m => m.GetToken()).Returns(() => _mockApiToken);
            _provider = provider.Object;
            _client = new AuthenticationClient(_httpClient, _settings, _provider);
        }

        [TestMethod]
        public async Task LoginTest()
        {
            _httpClient.AddResponse(ApiToken);

            Assert.IsTrue(string.IsNullOrEmpty(_provider.GetToken()));

            var username = DataGenerator.RandomUsername();
            var password = DataGenerator.RandomPassword();
            _ = await _client.AuthenticateAsync(username, password);

            Assert.AreEqual($"Bearer {ApiToken}", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.AreEqual($"{_settings.ApiUrl}", _httpClient.BaseAddress.ToString());
            Assert.AreEqual(HttpMethod.Post, _httpClient.Requests[0].Method);
            Assert.AreEqual(_settings.ApiRoutes[0].Route, _httpClient.Requests[0].Uri);

            Assert.AreEqual(ApiToken, _provider.GetToken());
        }

        [TestMethod]
        public async Task ClearAuthenticationTokenTest()
        {
            _httpClient.AddResponse(ApiToken);

            var username = DataGenerator.RandomUsername();
            var password = DataGenerator.RandomPassword();
            _ = await _client.AuthenticateAsync(username, password);

            Assert.AreEqual(ApiToken, _provider.GetToken());

            _client.ClearAuthentication();

            Assert.AreEqual($"Bearer", _httpClient.DefaultRequestHeaders.Authorization.ToString());
            Assert.IsTrue(string.IsNullOrEmpty(_provider.GetToken()));
        }
    }
}
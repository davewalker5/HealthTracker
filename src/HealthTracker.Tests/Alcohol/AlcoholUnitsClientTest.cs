using HealthTracker.Client.ApiClient;
using HealthTracker.Client.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.Alcohol
{
    [TestClass]
    public class AlcoholUnitsClientTest
    {
        private readonly string ApiToken = DataGenerator.RandomApiToken();
        private readonly MockHealthTrackerHttpClient _httpClient = new();
        private IAlcoholUnitCalculationsClient _client;

        private readonly HealthTrackerApplicationSettings _settings = new()
        {
            ShotSize = DataGenerator.RandomDecimal(25, 35),
            SmallGlassSize = 125M + DataGenerator.RandomDecimal(0, 75),
            MediumGlassSize = 175M + DataGenerator.RandomDecimal(0, 75),
            LargeGlassSize = 250M + DataGenerator.RandomDecimal(0, 75),
            ApiUrl = "http://server/",
            ApiRoutes = [
                new() { Name = "Alcohol", Route = "/alcohol" }
            ]
        };

        [TestInitialize]
        public void Initialise()
        {
            var provider = new Mock<IAuthenticationTokenProvider>();
            provider.Setup(x => x.GetToken()).Returns(ApiToken);
            _client = new AlcoholUnitCalculationsClient(_httpClient, _settings, provider.Object);
        }

        [TestMethod]
        public async Task UnitsTest()
        {
            var abv = DataGenerator.RandomDecimal(12, 40);
            var volume = DataGenerator.RandomDecimal(10, 250);
            var units = Math.Round(abv * volume / 1000M, 2, MidpointRounding.AwayFromZero);
            _httpClient.AddResponse(units.ToString());

            var actual = await _client.CalculateUnitsAsync(abv, volume);

            Assert.AreEqual(units, actual);
        }

        [TestMethod]
        public async Task UnitsPerShotTest()
        {
            var abv = DataGenerator.RandomDecimal(12, 40);
            var units = Math.Round(abv * _settings.ShotSize / 1000M, 2, MidpointRounding.AwayFromZero);
            _httpClient.AddResponse(units.ToString());

            var actual = await _client.UnitsPerShot(abv);

            Assert.AreEqual(units, actual);
        }

        [TestMethod]
        public async Task UnitsPerPintTest()
        {
            var abv = DataGenerator.RandomDecimal(3, 8);
            var units = Math.Round(abv * 568M / 1000M, 2, MidpointRounding.AwayFromZero);
            _httpClient.AddResponse(units.ToString());

            var actual = await _client.UnitsPerPint(abv);

            Assert.AreEqual(units, actual);
        }

        [TestMethod]
        public async Task UnitsPerSmallGlassTest()
        {
            var abv = DataGenerator.RandomDecimal(12, 40);
            var units = Math.Round(abv * _settings.SmallGlassSize / 1000M, 2, MidpointRounding.AwayFromZero);
            _httpClient.AddResponse(units.ToString());

            var actual = await _client.UnitsPerSmallGlass(abv);

            Assert.AreEqual(units, actual);
        }

        [TestMethod]
        public async Task UnitsPerMediumGlassTest()
        {
            var abv = DataGenerator.RandomDecimal(12, 40);
            var units = Math.Round(abv * _settings.MediumGlassSize / 1000M, 2, MidpointRounding.AwayFromZero);
            _httpClient.AddResponse(units.ToString());

            var actual = await _client.UnitsPerMediumGlass(abv);

            Assert.AreEqual(units, actual);
        }

        [TestMethod]
        public async Task UnitsPerLargeGlassTest()
        {
            var abv = DataGenerator.RandomDecimal(12, 40);
            var units = Math.Round(abv * _settings.LargeGlassSize / 1000M, 2, MidpointRounding.AwayFromZero);
            _httpClient.AddResponse(units.ToString());

            var actual = await _client.UnitsPerLargeGlass(abv);

            Assert.AreEqual(units, actual);
        }
    }
}
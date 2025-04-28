using HealthTracker.Data;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.Logic.Factory;
using Moq;

namespace HealthTracker.Tests.BloodOxygenSaturation
{
    [TestClass]
    public class BloodOxygenSaturationBandManagerTest
    {
        private readonly List<BloodOxygenSaturationBand> _bands = [
            new() { Name = "Normal", MinimumSPO2 = 93, MaximumSPO2 = decimal.MaxValue, MinimumAge = 0, MaximumAge = 12 },
            new() { Name = "Low", MinimumSPO2 = 0, MaximumSPO2 = 92.99M, MinimumAge = 0, MaximumAge = 12 },
            new() { Name = "Normal", MinimumSPO2 = 96, MaximumSPO2 = decimal.MaxValue, MinimumAge = 13, MaximumAge = 69 },
            new() { Name = "Low", MinimumSPO2 = 0, MaximumSPO2 = 95.99M, MinimumAge = 13, MaximumAge = 69 },
            new() { Name = "Normal", MinimumSPO2 = 94, MaximumSPO2 = decimal.MaxValue, MinimumAge = 70, MaximumAge = int.MaxValue },
            new() { Name = "Low", MinimumSPO2 = 0, MaximumSPO2 = 99.33M, MinimumAge = 70, MaximumAge = int.MaxValue },
        ];

        private IHealthTrackerFactory _factory;

        [TestInitialize]
        public async Task Initialise()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            foreach (var band in _bands)
            {
                await context.BloodOxygenSaturationBands.AddAsync(band);
            }
            await context.SaveChangesAsync();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
        }

        [TestMethod]
        public async Task ListBandsTest()
        {
            var bands = await _factory.BloodOxygenSaturationBands.ListAsync(x => true);
            Assert.AreEqual(6, bands.Count);
        }

        [TestMethod]
        public async Task BandValuesTest()
        {
            var bands = await _factory.BloodOxygenSaturationBands.ListAsync(x => (x.MinimumAge > 12) && (x.MaximumAge < 70) && (x.Name == "Normal"));
            Assert.AreEqual(1, bands.Count);
            Assert.AreEqual("Normal", bands[0].Name);
            Assert.AreEqual(13, bands[0].MinimumAge);
            Assert.AreEqual(69, bands[0].MaximumAge);
            Assert.AreEqual(96M, bands[0].MinimumSPO2);
            Assert.AreEqual(decimal.MaxValue, bands[0].MaximumSPO2);
        }
    }
}
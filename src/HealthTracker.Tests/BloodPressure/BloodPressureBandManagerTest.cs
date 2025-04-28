using HealthTracker.Data;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.Logic.Factory;
using Moq;

namespace HealthTracker.Tests.BloodPressure
{
    [TestClass]
    public class BloodPressureBandManagerTest
    {
        private readonly List<BloodPressureBand> _bands = [
            new() { Name = "Isolated Systolic Hypertension", MinimumSystolic = 140, MaximumSystolic = int.MaxValue, MinimumDiastolic = 0, MaximumDiastolic = 89, Order = 1, MatchAll = true },
            new() { Name = "Grade 3 Hypertension (Severe)", MinimumSystolic = 180, MaximumSystolic = int.MaxValue, MinimumDiastolic = 110, MaximumDiastolic = int.MaxValue, Order = 2, MatchAll = false },
            new() { Name = "Grade 2 Hypertension (Moderate)", MinimumSystolic = 160, MaximumSystolic = 179, MinimumDiastolic = 100, MaximumDiastolic = 109, Order = 3, MatchAll = false },
            new() { Name = "Grade 1 Hypertension (Mild)", MinimumSystolic = 140, MaximumSystolic = 159, MinimumDiastolic = 90, MaximumDiastolic = 99, Order = 4, MatchAll = false },
            new() { Name = "High Normal", MinimumSystolic = 130, MaximumSystolic = 139, MinimumDiastolic = 85, MaximumDiastolic = 89, Order = 5, MatchAll = false },
            new() { Name = "Normal", MinimumSystolic = 120, MaximumSystolic = 129, MinimumDiastolic = 80, MaximumDiastolic = 84, Order = 6, MatchAll = true },
            new() { Name = "Optimal", MinimumSystolic = 0, MaximumSystolic = 119, MinimumDiastolic = 0, MaximumDiastolic = 79, Order = 7, MatchAll = true },
        ];

        private IHealthTrackerFactory _factory;

        [TestInitialize]
        public async Task Initialise()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            foreach (var band in _bands)
            {
                await context.BloodPressureBands.AddAsync(band);
            }
            await context.SaveChangesAsync();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
        }

        [TestMethod]
        public async Task BandOrderTest()
        {
            var bands = await _factory.BloodPressureBands.ListAsync(x => true);
            Assert.AreEqual(7, bands.Count);
            Assert.AreEqual(1, bands[0].Order);
            Assert.AreEqual(2, bands[1].Order);
            Assert.AreEqual(3, bands[2].Order);
            Assert.AreEqual(4, bands[3].Order);
            Assert.AreEqual(5, bands[4].Order);
            Assert.AreEqual(6, bands[5].Order);
            Assert.AreEqual(7, bands[6].Order);
        }

        [TestMethod]
        public async Task BandValueTest()
        {
            var bands = await _factory.BloodPressureBands.ListAsync(x => true);
            Assert.AreEqual(7, bands.Count);
            Assert.AreEqual("Optimal", bands[6].Name);
            Assert.AreEqual(0, bands[6].MinimumSystolic);
            Assert.AreEqual(119, bands[6].MaximumSystolic);
            Assert.AreEqual(0, bands[6].MinimumDiastolic);
            Assert.AreEqual(79, bands[6].MaximumDiastolic);
            Assert.AreEqual(7, bands[6].Order);
            Assert.IsTrue(bands[6].MatchAll);
        }
    }
}
using HealthTracker.Data;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.BeverageMeasures
{
    [TestClass]
    public class BeverageMeasureManagerTest
    {
        private readonly string Name = DataGenerator.RandomTitleCasePhrase(2, 5, 10);
        private readonly decimal Volume = DataGenerator.RandomDecimal(25, 500);
        private readonly string UpdatedName = DataGenerator.RandomTitleCasePhrase(2, 5, 10);
        private readonly decimal UpdatedVolume = DataGenerator.RandomDecimal(25, 500);

        private IHealthTrackerFactory _factory;
        private int _beverageMeasureId;

        [TestInitialize]
        public void TestInitialize()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            _beverageMeasureId = Task.Run(() => _factory.BeverageMeasures.AddAsync(Name, Volume)).Result.Id;
        }

        [TestMethod]
        public async Task AddAndGetTest()
        {
            var beverageMeasure = await _factory.BeverageMeasures.GetAsync(a => a.Id == _beverageMeasureId);
            Assert.IsNotNull(beverageMeasure);
            Assert.AreEqual(_beverageMeasureId, beverageMeasure.Id);
            Assert.AreEqual(Name, beverageMeasure.Name);
            Assert.AreEqual(Volume, beverageMeasure.Volume);
        }

        [TestMethod]
        public async Task GetMissingTest()
        {
            var beverageMeasure = await _factory.BeverageMeasures.GetAsync(a => a.Id == (10 * _beverageMeasureId));
            Assert.IsNull(beverageMeasure);
        }

        [TestMethod]
        public async Task ListAllTest()
        {
            var beverageMeasures = await _factory.BeverageMeasures.ListAsync(x => true, 1, int.MaxValue);
            Assert.AreEqual(1, beverageMeasures.Count);
            Assert.AreEqual(Name, beverageMeasures.First().Name);
            Assert.AreEqual(Volume, beverageMeasures.First().Volume);
        }

        [TestMethod]
        public async Task ListMissingTest()
        {
            var beverageMeasures = await _factory.BeverageMeasures.ListAsync(e => e.Name == "Missing", 1, int.MaxValue);
            Assert.AreEqual(0, beverageMeasures.Count);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            await _factory.BeverageMeasures.UpdateAsync(_beverageMeasureId, UpdatedName, UpdatedVolume);
            var beverageMeasure = await _factory.BeverageMeasures.GetAsync(a => a.Id == _beverageMeasureId);
            Assert.IsNotNull(beverageMeasure);
            Assert.AreEqual(_beverageMeasureId, beverageMeasure.Id);
            Assert.AreEqual(UpdatedName, beverageMeasure.Name);
            Assert.AreEqual(UpdatedVolume, beverageMeasure.Volume);
            Assert.AreNotEqual(Name, UpdatedName);
            Assert.AreNotEqual(Volume, UpdatedVolume);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            await _factory.BeverageMeasures.DeleteAsync(_beverageMeasureId);
            var beverageMeasures = await _factory.BeverageMeasures.ListAsync(a => true, 1, int.MaxValue);
            Assert.AreEqual(0, beverageMeasures.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(BeverageMeasureExistsException))]
        public async Task CannotAddDuplicateBeverageMeasureTest()
            => _ = await _factory.BeverageMeasures.AddAsync(Name, 0M);
    }
}
using HealthTracker.Data;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.FoodSources
{
    [TestClass]
    public class FoodSourceManagerTest
    {
        private readonly string Name = DataGenerator.RandomTitleCasePhrase(3, 5, 15);
        private readonly string UpdatedName = DataGenerator.RandomTitleCasePhrase(3, 5, 15);

        private IHealthTrackerFactory _factory;
        private int _foodSourceId;

        [TestInitialize]
        public void TestInitialize()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            _foodSourceId = Task.Run(() => _factory.FoodSources.AddAsync(Name)).Result.Id;
        }

        [TestMethod]
        public async Task AddAndGetTest()
        {
            var foodSource = await _factory.FoodSources.GetAsync(a => a.Id == _foodSourceId);
            Assert.IsNotNull(foodSource);
            Assert.AreEqual(_foodSourceId, foodSource.Id);
            Assert.AreEqual(Name, foodSource.Name);
        }

        [TestMethod]
        public async Task GetMissingTest()
        {
            var foodSource = await _factory.FoodSources.GetAsync(a => a.Id == (10 * _foodSourceId));
            Assert.IsNull(foodSource);
        }

        [TestMethod]
        public async Task ListAllTest()
        {
            var foodSources = await _factory.FoodSources.ListAsync(x => true, 1, int.MaxValue);
            Assert.AreEqual(1, foodSources.Count);
            Assert.AreEqual(Name, foodSources.First().Name);
        }

        [TestMethod]
        public async Task ListMissingTest()
        {
            var foodSources = await _factory.FoodSources.ListAsync(e => e.Name == "Missing", 1, int.MaxValue);
            Assert.AreEqual(0, foodSources.Count);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            await _factory.FoodSources.UpdateAsync(_foodSourceId, UpdatedName);
            var foodSource = await _factory.FoodSources.GetAsync(a => a.Id == _foodSourceId);
            Assert.IsNotNull(foodSource);
            Assert.AreEqual(_foodSourceId, foodSource.Id);
            Assert.AreEqual(UpdatedName, foodSource.Name);
            Assert.AreNotEqual(Name, UpdatedName);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            await _factory.FoodSources.DeleteAsync(_foodSourceId);
            var foodSources = await _factory.FoodSources.ListAsync(a => true, 1, int.MaxValue);
            Assert.AreEqual(0, foodSources.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(FoodSourceExistsException))]
        public async Task CannotAddDuplicateFoodSourceTest()
            => _ = await _factory.FoodSources.AddAsync(Name);

        [TestMethod]
        [ExpectedException(typeof(FoodSourceInUseException))]
        public void CannotDeleteWithFoodConsumptionMeasurementTest()
        {
            // TODO: Implement once food consumption measurements are implemented
            throw new FoodSourceInUseException();
        }
    }
}
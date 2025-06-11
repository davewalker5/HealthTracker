using HealthTracker.Data;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Food;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.Beverages
{
    [TestClass]
    public class BeverageManagerTest
    {
        private readonly string Name = DataGenerator.RandomTitleCasePhrase(2, 5, 10);
        private readonly decimal TypicalABV = DataGenerator.RandomDecimal(0, 40);
        private readonly bool IsHydrating = true;
        private readonly bool IsAlcohol = false;
        private readonly string UpdatedName = DataGenerator.RandomTitleCasePhrase(2, 5, 10);
        private readonly decimal UpdatedTypicalABV = DataGenerator.RandomDecimal(0, 40);
        private readonly bool UpdatedIsHydrating = false;
        private readonly bool UpdatedIsAlcohol = true;

        private IHealthTrackerFactory _factory;
        private int _beverageId;

        [TestInitialize]
        public void TestInitialize()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            _beverageId = Task.Run(() => _factory.Beverages.AddAsync(Name, TypicalABV, IsHydrating, IsAlcohol)).Result.Id;
        }

        [TestMethod]
        public async Task AddAndGetTest()
        {
            var beverage = await _factory.Beverages.GetAsync(a => a.Id == _beverageId);
            Assert.IsNotNull(beverage);
            Assert.AreEqual(_beverageId, beverage.Id);
            Assert.AreEqual(Name, beverage.Name);
            Assert.AreEqual(TypicalABV, beverage.TypicalABV);
            Assert.AreEqual(IsHydrating, beverage.IsHydrating);
            Assert.AreEqual(IsAlcohol, beverage.IsAlcohol);
        }

        [TestMethod]
        public async Task GetMissingTest()
        {
            var beverage = await _factory.Beverages.GetAsync(a => a.Id == (10 * _beverageId));
            Assert.IsNull(beverage);
        }

        [TestMethod]
        public async Task ListAllTest()
        {
            var beverages = await _factory.Beverages.ListAsync(x => true, 1, int.MaxValue);
            Assert.AreEqual(1, beverages.Count);
            Assert.AreEqual(Name, beverages.First().Name);
            Assert.AreEqual(TypicalABV, beverages.First().TypicalABV);
            Assert.AreEqual(IsHydrating, beverages.First().IsHydrating);
            Assert.AreEqual(IsAlcohol, beverages.First().IsAlcohol);
        }

        [TestMethod]
        public async Task ListMissingTest()
        {
            var beverages = await _factory.Beverages.ListAsync(e => e.Name == "Missing", 1, int.MaxValue);
            Assert.AreEqual(0, beverages.Count);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            await _factory.Beverages.UpdateAsync(_beverageId, UpdatedName, UpdatedTypicalABV, UpdatedIsHydrating, UpdatedIsAlcohol);
            var beverage = await _factory.Beverages.GetAsync(a => a.Id == _beverageId);
            Assert.IsNotNull(beverage);
            Assert.AreEqual(_beverageId, beverage.Id);
            Assert.AreEqual(UpdatedName, beverage.Name);
            Assert.AreEqual(UpdatedTypicalABV, beverage.TypicalABV);
            Assert.AreNotEqual(Name, UpdatedName);
            Assert.AreNotEqual(TypicalABV, UpdatedTypicalABV);
            Assert.AreEqual(UpdatedIsHydrating, beverage.IsHydrating);
            Assert.AreEqual(UpdatedIsAlcohol, beverage.IsAlcohol);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            await _factory.Beverages.DeleteAsync(_beverageId);
            var beverages = await _factory.Beverages.ListAsync(a => true, 1, int.MaxValue);
            Assert.AreEqual(0, beverages.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(BeverageExistsException))]
        public async Task CannotAddDuplicateBeverageTest()
            => _ = await _factory.Beverages.AddAsync(Name, 0M, false, false);

        [TestMethod]
        [ExpectedException(typeof(BeverageInUseException))]
        public async Task CannotDeleteWithAlcoholConsumptionMeasurementTest()
        {
            var context = _factory.Context as HealthTrackerDbContext;
            await context.BeverageConsumptionMeasurements.AddAsync(new BeverageConsumptionMeasurement
            {
                BeverageId = _beverageId
            });
            await context.SaveChangesAsync();
            await _factory.Beverages.DeleteAsync(_beverageId);
        }
    }
}
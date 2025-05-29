using HealthTracker.Data;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.Alcohol
{
    [TestClass]
    public class BeverageManagerTest
    {
        private readonly string Name = DataGenerator.RandomTitleCasePhrase(2, 5, 10);
        private readonly decimal TypicalABV = DataGenerator.RandomDecimal(0, 40);
        private readonly string UpdatedName = DataGenerator.RandomTitleCasePhrase(2, 5, 10);
        private readonly decimal UpdatedTypicalABV = DataGenerator.RandomDecimal(0, 40);

        private IHealthTrackerFactory _factory;
        private int _beverageId;

        [TestInitialize]
        public void TestInitialize()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            _beverageId = Task.Run(() => _factory.Beverages.AddAsync(Name, TypicalABV)).Result.Id;
        }

        [TestMethod]
        public async Task AddAndGetTest()
        {
            var beverage = await _factory.Beverages.GetAsync(a => a.Id == _beverageId);
            Assert.IsNotNull(beverage);
            Assert.AreEqual(_beverageId, beverage.Id);
            Assert.AreEqual(Name, beverage.Name);
            Assert.AreEqual(TypicalABV, beverage.TypicalABV);
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
            await _factory.Beverages.UpdateAsync(_beverageId, UpdatedName, UpdatedTypicalABV);
            var beverage = await _factory.Beverages.GetAsync(a => a.Id == _beverageId);
            Assert.IsNotNull(beverage);
            Assert.AreEqual(_beverageId, beverage.Id);
            Assert.AreEqual(UpdatedName, beverage.Name);
            Assert.AreEqual(UpdatedTypicalABV, beverage.TypicalABV);
            Assert.AreNotEqual(Name, UpdatedName);
            Assert.AreNotEqual(TypicalABV, UpdatedTypicalABV);
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
            => _ = await _factory.Beverages.AddAsync(Name, 0M);

        // TODO: Implement this once alcohol consumption records are in
        // [TestMethod]
        // [ExpectedException(typeof(BeverageInUseException))]
        // public async Task CannotDeleteWithExerciseMeasurementTest()
        // {
        //     var context = _factory.Context as HealthTrackerDbContext;
        //     await context.AlcoholConsumptionMeasurements.AddAsync(new AlcoholConsumptionMeasurement
        //     {
        //     });
        //     await context.SaveChangesAsync();
        //     await _factory.Beverages.DeleteAsync(_beverageId);
        // }
    }
}
using HealthTracker.Data;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.Meals
{
    [TestClass]
    public class MealManagerTest
    {
        private readonly string Name = DataGenerator.RandomTitleCasePhrase(5, 5, 20);
        private readonly int Portions = DataGenerator.RandomInt(1, 10);
        private readonly string Reference = DataGenerator.RandomTitleCasePhrase(2, 5, 10);

        private readonly string UpdatedName = DataGenerator.RandomTitleCasePhrase(5, 5, 20);
        private readonly int UpdatedPortions = DataGenerator.RandomInt(1, 10);
        private readonly string UpdatedReference = DataGenerator.RandomTitleCasePhrase(2, 5, 10);

        private IHealthTrackerFactory _factory;
        private int _sourceId;
        private int _updatedSourceId;
        private int _nutritionalValueId;
        private int _mealId;

        [TestInitialize]
        public void TestInitialize()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            _sourceId = Task.Run(() => _factory.FoodSources.AddAsync(DataGenerator.RandomTitleCasePhrase(3, 5, 10))).Result.Id;
            _updatedSourceId = Task.Run(() => _factory.FoodSources.AddAsync(DataGenerator.RandomTitleCasePhrase(4, 5, 10))).Result.Id;
            _nutritionalValueId = Task.Run(() => _factory.NutritionalValues.AddAsync(
                DataGenerator.RandomDecimal(10, 100),
                DataGenerator.RandomDecimal(0, 100),
                DataGenerator.RandomDecimal(0, 100),
                DataGenerator.RandomDecimal(0, 100),
                DataGenerator.RandomDecimal(0, 100),
                DataGenerator.RandomDecimal(0, 100),
                DataGenerator.RandomDecimal(0, 100)
            )).Result.Id;
            _mealId = Task.Run(() => _factory.Meals.AddAsync(Name, Portions, _sourceId, Reference, _nutritionalValueId)).Result.Id;
        }

        [TestMethod]
        public async Task AddAndListTest()
        {
            var meals = await _factory.Meals.ListAsync(x => x.Id == _mealId, 1, int.MaxValue);
            Assert.AreEqual(1, meals.Count);
            Assert.AreEqual(_mealId, meals.First().Id);
            Assert.AreEqual(_sourceId, meals.First().FoodSourceId);
            Assert.AreEqual(Reference, meals.First().Reference);
            Assert.AreEqual(_nutritionalValueId, meals.First().NutritionalValueId);
            Assert.AreEqual(Name, meals.First().Name);
            Assert.AreEqual(Portions, meals.First().Portions);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            await _factory.Meals.UpdateAsync(_mealId, UpdatedName, UpdatedPortions, _updatedSourceId, UpdatedReference, null);
            var meals = await _factory.Meals.ListAsync(x => x.Id == _mealId, 1, int.MaxValue);
            Assert.AreEqual(1, meals.Count);
            Assert.AreEqual(_mealId, meals.First().Id);
            Assert.AreEqual(_updatedSourceId, meals.First().FoodSourceId);
            Assert.AreEqual(UpdatedReference, meals.First().Reference);
            Assert.IsNull(meals.First().NutritionalValueId);
            Assert.AreEqual(UpdatedName, meals.First().Name);
            Assert.AreEqual(UpdatedPortions, meals.First().Portions);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            await _factory.Meals.DeleteAsync(_mealId);
            var meals = await _factory.Meals.ListAsync(a => true, 1, int.MaxValue);
            Assert.AreEqual(0, meals.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(MealExistsException))]
        public async Task CannotCreateDuplicateTest()
            => await _factory.Meals.AddAsync(Name, UpdatedPortions, _updatedSourceId, UpdatedReference, null);

        [TestMethod]
        [ExpectedException(typeof(FoodSourceNotFoundException))]
        public async Task CannotAddMealForMissingFoodSourceTest()
            => await _factory.Meals.AddAsync(UpdatedName, UpdatedPortions, 10 * _sourceId, UpdatedReference, null);

        [TestMethod]
        [ExpectedException(typeof(NutritionalValueNotFoundException))]
        public async Task CannotUpdateMealForMissingNutritionalValueTest()
            => await _factory.Meals.UpdateAsync(_mealId, UpdatedName, UpdatedPortions, _updatedSourceId, UpdatedReference, 10 * _nutritionalValueId);
    }
}

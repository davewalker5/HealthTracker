using HealthTracker.Data;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.FoodItems
{
    [TestClass]
    public class FoodItemManagerTest
    {
        private readonly string Name = DataGenerator.RandomTitleCasePhrase(5, 5, 20);
        private readonly decimal Portion = DataGenerator.RandomDecimal(100, 500);

        private readonly string UpdatedName = DataGenerator.RandomTitleCasePhrase(5, 5, 20);
        private readonly decimal UpdatedPortion = DataGenerator.RandomDecimal(100, 500);

        private IHealthTrackerFactory _factory;
        private int _categoryId;
        private int _updatedCategoryId;
        private int _nutritionalValueId;
        private int _itemId;

        [TestInitialize]
        public void TestInitialize()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            _categoryId = Task.Run(() => _factory.FoodCategories.AddAsync(DataGenerator.RandomTitleCasePhrase(3, 5, 10))).Result.Id;
            _updatedCategoryId = Task.Run(() => _factory.FoodCategories.AddAsync(DataGenerator.RandomTitleCasePhrase(4, 5, 10))).Result.Id;
            _nutritionalValueId = Task.Run(() => _factory.NutritionalValues.AddAsync(
                DataGenerator.RandomDecimal(10, 100),
                DataGenerator.RandomDecimal(0, 100),
                DataGenerator.RandomDecimal(0, 100),
                DataGenerator.RandomDecimal(0, 100),
                DataGenerator.RandomDecimal(0, 100),
                DataGenerator.RandomDecimal(0, 100),
                DataGenerator.RandomDecimal(0, 100)
            )).Result.Id;
            _itemId = Task.Run(() => _factory.FoodItems.AddAsync(Name, Portion, _categoryId, _nutritionalValueId)).Result.Id;
        }

        [TestMethod]
        public async Task AddAndListTest()
        {
            var items = await _factory.FoodItems.ListAsync(x => x.Id == _itemId, 1, int.MaxValue);
            Assert.AreEqual(1, items.Count);
            Assert.AreEqual(_itemId, items.First().Id);
            Assert.AreEqual(_categoryId, items.First().FoodCategoryId);
            Assert.AreEqual(_nutritionalValueId, items.First().NutritionalValueId);
            Assert.AreEqual(Name, items.First().Name);
            Assert.AreEqual(Portion, items.First().Portion);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            await _factory.FoodItems.UpdateAsync(_itemId, UpdatedName, UpdatedPortion, _updatedCategoryId, null);
            var items = await _factory.FoodItems.ListAsync(x => x.Id == _itemId, 1, int.MaxValue);
            Assert.AreEqual(1, items.Count);
            Assert.AreEqual(_itemId, items.First().Id);
            Assert.AreEqual(_updatedCategoryId, items.First().FoodCategoryId);
            Assert.IsNull(items.First().NutritionalValueId);
            Assert.AreEqual(UpdatedName, items.First().Name);
            Assert.AreEqual(UpdatedPortion, items.First().Portion);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            await _factory.FoodItems.DeleteAsync(_itemId);
            var items = await _factory.FoodItems.ListAsync(a => true, 1, int.MaxValue);
            Assert.AreEqual(0, items.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(FoodItemExistsException))]
        public async Task CannotCreateDuplicateTest()
            => await _factory.FoodItems.AddAsync(Name, UpdatedPortion, _categoryId, null);

        [TestMethod]
        [ExpectedException(typeof(FoodCategoryNotFoundException))]
        public async Task CannotAddItemForMissingFoodCategoryTest()
            => await _factory.FoodItems.AddAsync(UpdatedName, UpdatedPortion, 10 * _categoryId, null);

        [TestMethod]
        [ExpectedException(typeof(NutritionalValueNotFoundException))]
        public async Task CannotUpdateItemForMissingNutritionalValueTest()
            => await _factory.FoodItems.UpdateAsync(_itemId, UpdatedName, UpdatedPortion, _categoryId, 10 * _nutritionalValueId);
    }
}

using HealthTracker.Data;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.NutritionalValues
{
    [TestClass]
    public class NutritionalValueManagerTest
    {
        private readonly decimal Calories = DataGenerator.RandomDecimal(10, 100);
        private readonly decimal Fat = DataGenerator.RandomDecimal(0, 100);
        private readonly decimal SaturatedFat = DataGenerator.RandomDecimal(0, 100);
        private readonly decimal Protein = DataGenerator.RandomDecimal(0, 100);
        private readonly decimal Carbohydrates = DataGenerator.RandomDecimal(0, 100);
        private readonly decimal Sugar = DataGenerator.RandomDecimal(0, 100);
        private readonly decimal Fibre = DataGenerator.RandomDecimal(0, 100);

        private readonly decimal UpdatedCalories = DataGenerator.RandomDecimal(10, 100);
        private readonly decimal UpdatedFat = DataGenerator.RandomDecimal(0, 100);
        private readonly decimal UpdatedSaturatedFat = DataGenerator.RandomDecimal(0, 100);
        private readonly decimal UpdatedProtein = DataGenerator.RandomDecimal(0, 100);
        private readonly decimal UpdatedCarbohydrates = DataGenerator.RandomDecimal(0, 100);
        private readonly decimal UpdatedSugar = DataGenerator.RandomDecimal(0, 100);
        private readonly decimal UpdatedFibre = DataGenerator.RandomDecimal(0, 100);

        private IHealthTrackerFactory _factory;
        private int _nutritionalValueId;

        [TestInitialize]
        public void TestInitialize()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            _nutritionalValueId = Task.Run(() => _factory.NutritionalValues.AddAsync(Calories, Fat, SaturatedFat, Protein, Carbohydrates, Sugar, Fibre)).Result.Id;
        }

        [TestMethod]
        public async Task AddAndGetTest()
        {
            var nutritionalValue = await _factory.NutritionalValues.GetAsync(a => a.Id == _nutritionalValueId);
            Assert.IsNotNull(nutritionalValue);
            Assert.AreEqual(_nutritionalValueId, nutritionalValue.Id);
            Assert.AreEqual(Calories, nutritionalValue.Calories);
            Assert.AreEqual(Fat, nutritionalValue.Fat);
            Assert.AreEqual(SaturatedFat, nutritionalValue.SaturatedFat);
            Assert.AreEqual(Protein, nutritionalValue.Protein);
            Assert.AreEqual(Carbohydrates, nutritionalValue.Carbohydrates);
            Assert.AreEqual(Sugar, nutritionalValue.Sugar);
            Assert.AreEqual(Fibre, nutritionalValue.Fibre);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            await _factory.NutritionalValues.UpdateAsync(_nutritionalValueId, UpdatedCalories, UpdatedFat, UpdatedSaturatedFat, UpdatedProtein, UpdatedCarbohydrates, UpdatedSugar, UpdatedFibre);
            var nutritionalValue = await _factory.NutritionalValues.GetAsync(a => a.Id == _nutritionalValueId);
            Assert.IsNotNull(nutritionalValue);
            Assert.AreEqual(_nutritionalValueId, nutritionalValue.Id);
            Assert.AreEqual(UpdatedCalories, nutritionalValue.Calories);
            Assert.AreEqual(UpdatedFat, nutritionalValue.Fat);
            Assert.AreEqual(UpdatedSaturatedFat, nutritionalValue.SaturatedFat);
            Assert.AreEqual(UpdatedProtein, nutritionalValue.Protein);
            Assert.AreEqual(UpdatedCarbohydrates, nutritionalValue.Carbohydrates);
            Assert.AreEqual(UpdatedSugar, nutritionalValue.Sugar);
            Assert.AreEqual(UpdatedFibre, nutritionalValue.Fibre);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            await _factory.NutritionalValues.DeleteAsync(_nutritionalValueId);
            var nutritionalValue = await _factory.NutritionalValues.GetAsync(a => a.Id == _nutritionalValueId);
            Assert.IsNull(nutritionalValue);
        }
    }
}
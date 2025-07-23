using HealthTracker.Data;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Food;
using HealthTracker.Enumerations.Enumerations;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.MealConsumption
{
    [TestClass]
    public class MealConsumptionMeasurementManagerTest
    {
        private readonly DateTime ConsumptionDate = DataGenerator.RandomDateInYear(2024);
        private readonly decimal Quantity = DataGenerator.RandomDecimal(1, 100);

        private readonly DateTime UpdatedConsumptionDate = DataGenerator.RandomDateInYear(2024);
        private readonly decimal UpdatedQuantity = DataGenerator.RandomDecimal(1, 100);

        private IHealthTrackerFactory _factory;
        private int _personId;
        private int _mealId;
        private int _measurementId;
        private NutritionalValue _nutrition;

        [TestInitialize]
        public void TestInitialize()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            _personId = Task.Run(() => _factory.People.AddAsync("", "", DateTime.Now, 0, Gender.Unspecified)).Result.Id;
            var foodSourceId = Task.Run(() => _factory.FoodSources.AddAsync("My Favourite Restaurant")).Result.Id;
            _nutrition = Task.Run(() => _factory.NutritionalValues.AddAsync(
                DataGenerator.RandomDecimal(10, 100),
                DataGenerator.RandomDecimal(0, 100),
                DataGenerator.RandomDecimal(0, 100),
                DataGenerator.RandomDecimal(0, 100),
                DataGenerator.RandomDecimal(0, 100),
                DataGenerator.RandomDecimal(0, 100),
                DataGenerator.RandomDecimal(0, 100)
            )).Result;
            _mealId = Task.Run(() => _factory.Meals.AddAsync("Some Meal", 1, foodSourceId, _nutrition.Id)).Result.Id;
            _measurementId = Task.Run(() => _factory.MealConsumptionMeasurements.AddAsync(_personId, _mealId, ConsumptionDate, Quantity)).Result.Id;
        }

        [TestMethod]
        public async Task AddAndListTest()
        {
            var measurements = await _factory.MealConsumptionMeasurements.ListAsync(x => x.PersonId == _personId, 1, int.MaxValue);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(_measurementId, measurements.First().Id);
            Assert.AreEqual(_personId, measurements.First().PersonId);
            Assert.AreEqual(_mealId, measurements.First().MealId);
            Assert.AreEqual(Quantity * _nutrition.Calories, measurements.First().NutritionalValue.Calories);
            Assert.AreEqual(Quantity * _nutrition.Fat, measurements.First().NutritionalValue.Fat);
            Assert.AreEqual(Quantity * _nutrition.SaturatedFat, measurements.First().NutritionalValue.SaturatedFat);
            Assert.AreEqual(Quantity * _nutrition.Protein, measurements.First().NutritionalValue.Protein);
            Assert.AreEqual(Quantity * _nutrition.Carbohydrates, measurements.First().NutritionalValue.Carbohydrates);
            Assert.AreEqual(Quantity * _nutrition.Sugar, measurements.First().NutritionalValue.Sugar);
            Assert.AreEqual(Quantity * _nutrition.Fibre, measurements.First().NutritionalValue.Fibre);
            Assert.AreEqual(ConsumptionDate, measurements.First().Date);
            Assert.AreEqual(Quantity, measurements.First().Quantity);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            await _factory.MealConsumptionMeasurements.UpdateAsync(_measurementId, _personId, _mealId, UpdatedConsumptionDate, UpdatedQuantity);
            var measurements = await _factory.MealConsumptionMeasurements.ListAsync(x => x.PersonId == _personId, 1, int.MaxValue);
            Console.WriteLine(measurements.First());
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(_measurementId, measurements.First().Id);
            Assert.AreEqual(_personId, measurements.First().PersonId);
            Assert.AreEqual(_mealId, measurements.First().MealId);
            Assert.AreEqual(UpdatedQuantity * _nutrition.Calories, measurements.First().NutritionalValue.Calories);
            Assert.AreEqual(UpdatedQuantity * _nutrition.Fat, measurements.First().NutritionalValue.Fat);
            Assert.AreEqual(UpdatedQuantity * _nutrition.SaturatedFat, measurements.First().NutritionalValue.SaturatedFat);
            Assert.AreEqual(UpdatedQuantity * _nutrition.Protein, measurements.First().NutritionalValue.Protein);
            Assert.AreEqual(UpdatedQuantity * _nutrition.Carbohydrates, measurements.First().NutritionalValue.Carbohydrates);
            Assert.AreEqual(UpdatedQuantity * _nutrition.Sugar, measurements.First().NutritionalValue.Sugar);
            Assert.AreEqual(UpdatedQuantity * _nutrition.Fibre, measurements.First().NutritionalValue.Fibre);
            Assert.AreEqual(UpdatedConsumptionDate, measurements.First().Date);
            Assert.AreEqual(UpdatedQuantity, measurements.First().Quantity);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            await _factory.MealConsumptionMeasurements.DeleteAsync(_measurementId);
            var measurements = await _factory.MealConsumptionMeasurements.ListAsync(a => true, 1, int.MaxValue);
            Assert.AreEqual(0, measurements.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(PersonNotFoundException))]
        public async Task CannotAddMeasurementForMissingPersonTest()
            => await _factory.MealConsumptionMeasurements.AddAsync(10 * _personId, _mealId, ConsumptionDate, Quantity);

        [TestMethod]
        [ExpectedException(typeof(PersonNotFoundException))]
        public async Task CannotUpdateMeasurementForMissingPersonTest()
            => await _factory.MealConsumptionMeasurements.UpdateAsync(_measurementId, 10 * _personId, _mealId, UpdatedConsumptionDate, UpdatedQuantity);

        [TestMethod]
        [ExpectedException(typeof(MealNotFoundException))]
        public async Task CannotAddMeasurementForMissingMealTest()
            => await _factory.MealConsumptionMeasurements.AddAsync(_personId, 10 * _mealId, ConsumptionDate, Quantity);

        [TestMethod]
        [ExpectedException(typeof(MealNotFoundException))]
        public async Task CannotUpdateMeasurementForMissingMealTest()
            => await _factory.MealConsumptionMeasurements.UpdateAsync(_measurementId, _personId, 10 * _mealId, UpdatedConsumptionDate, UpdatedQuantity);
    }
}

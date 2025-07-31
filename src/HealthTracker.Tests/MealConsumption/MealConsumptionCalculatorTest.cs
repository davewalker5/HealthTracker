using HealthTracker.Data;
using HealthTracker.Entities.Food;
using HealthTracker.Enumerations.Enumerations;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;
using HealthTracker.Entities.Identity;

namespace HealthTracker.Tests.MealConsumption
{
    [TestClass]
    public class MealConsumptionCalculatorTest
    {
        private readonly int Portions = DataGenerator.RandomInt(1, 10);
        private readonly DateTime ConsumptionDate = DataGenerator.RandomDateInYear(2024);
        private readonly decimal Quantity = DataGenerator.RandomDecimal(1, 100);

        private IHealthTrackerFactory _factory;
        private Person _person;
        private MealConsumptionMeasurement _measurement;

        [TestInitialize]
        public async Task TestInitialize()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);

            // Set up a meal
            var nutritionalValue = await _factory.NutritionalValues.AddAsync(DataGenerator.RandomNutritionalValue(0));
            var source = await _factory.FoodSources.AddAsync(DataGenerator.RandomTitleCasePhrase(3, 5, 10));
            var meal = await _factory.Meals.AddAsync(DataGenerator.RandomTitleCasePhrase(3, 5, 10), Portions, source.Id, null, nutritionalValue.Id);

            // Set up a person to have consumption records
            _person = await _factory.People.AddAsync("", "", DateTime.Now, 0, Gender.Unspecified);

            // Set up a measurement
            _measurement = await _factory.MealConsumptionMeasurements.AddAsync(_person.Id, meal.Id, ConsumptionDate, Quantity);
        }

        [TestMethod]
        public async Task GetDailyTotalsTest()
        {
            var from = _measurement.Date.AddDays(-1);
            var to = _measurement.Date.AddDays(1);
            var totals = await _factory.MealConsumptionCalculator.DailyTotalConsumptionAsync(_person.Id, from, to);

            Assert.AreEqual(1, totals.Count);
            Assert.AreEqual(_person.Name, totals.First().PersonName);
            Assert.AreEqual(_measurement.NutritionalValue.Calories, totals.First().Calories);
            Assert.AreEqual(_measurement.NutritionalValue.Fat, totals.First().Fat);
            Assert.AreEqual(_measurement.NutritionalValue.SaturatedFat, totals.First().SaturatedFat);
            Assert.AreEqual(_measurement.NutritionalValue.Protein, totals.First().Protein);
            Assert.AreEqual(_measurement.NutritionalValue.Carbohydrates, totals.First().Carbohydrates);
            Assert.AreEqual(_measurement.NutritionalValue.Sugar, totals.First().Sugar);
            Assert.AreEqual(_measurement.NutritionalValue.Fibre, totals.First().Fibre);
        }
    }
}

using HealthTracker.Data;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Enumerations.Enumerations;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;
using HealthTracker.Entities.Food;
using HealthTracker.Entities.Identity;

namespace HealthTracker.Tests.BeverageConsumption
{
    [TestClass]
    public class BeverageConsumptionCalculatorManagerTest
    {
        private IHealthTrackerFactory _factory;
        private Person _person;
        private Beverage _alcoholicBeverage;
        private Beverage _nonAlcoholicBeverage;

        [TestInitialize]
        public async Task TestInitialize()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);

            // Add a person to have measurements
            _person = await _factory.People.AddAsync("", "", DateTime.Now, 0, Gender.Unspecified);

            // Create one alcoholic and one non-alcoholic beverage
            _alcoholicBeverage = await _factory.Beverages.AddAsync("White Wine", DataGenerator.RandomDecimal(1, 40), false, true);
            _nonAlcoholicBeverage = await _factory.Beverages.AddAsync("Water", 0, true, false);
        }

        [TestMethod]
        public async Task DailyTotalHydratingAsyncTest()
        {
            var date = DataGenerator.RandomDateInYear(2024);
            var quantity = DataGenerator.RandomInt(1, 10);
            var volume = DataGenerator.RandomDecimal(25, 250);
            _ = _factory.BeverageConsumptionMeasurements.AddAsync(_person.Id, _nonAlcoholicBeverage.Id, date, quantity, volume, _nonAlcoholicBeverage.TypicalABV);
            var totals = await _factory.BeverageConsumptionCalculator.DailyTotalHydratingAsync(_person.Id, date.AddDays(-1), date.AddDays(1));

            Assert.AreEqual(1, totals.Count);
            Assert.AreEqual(_person.Id, totals.First().PersonId);
            Assert.AreEqual(_nonAlcoholicBeverage.Id, totals.First().BeverageId);
            Assert.AreEqual(date, totals.First().Date);
            Assert.AreEqual(_nonAlcoholicBeverage.TypicalABV, totals.First().ABV);
            Assert.AreEqual(volume, totals.First().Volume);
            Assert.AreEqual(quantity, totals.First().Quantity);
        }

        [TestMethod]
        public async Task TotalHydratingAsyncTest()
        {
            var date = DataGenerator.RandomDateInYear(2024);
            var from = date.AddDays(-1);
            var to = date.AddDays(1);
            var quantity = DataGenerator.RandomInt(1, 10);
            var volume = DataGenerator.RandomDecimal(25, 250);
            var expectedUnits = _factory.AlcoholUnitsCalculator.CalculateUnits(_nonAlcoholicBeverage.TypicalABV, volume * quantity);

            _ = _factory.BeverageConsumptionMeasurements.AddAsync(_person.Id, _nonAlcoholicBeverage.Id, date, quantity, volume, _nonAlcoholicBeverage.TypicalABV);
            var total = await _factory.BeverageConsumptionCalculator.TotalHydratingAsync(_person.Id, from, to);

            Assert.AreEqual(_person.Id, total.PersonId);
            Assert.AreEqual(_person.Name, total.PersonName);
            Assert.AreEqual(from, total.From);
            Assert.AreEqual(to, total.To);
            Assert.AreEqual(_nonAlcoholicBeverage.Id, total.BeverageId);
            Assert.AreEqual(_nonAlcoholicBeverage.Name, total.BeverageName);
            Assert.AreEqual(quantity * volume, total.TotalVolume);
            Assert.AreEqual(expectedUnits, total.TotalUnits);
        }

        [TestMethod]
        public async Task DailyTotalAlcoholAsyncTest()
        {
            var date = DataGenerator.RandomDateInYear(2024);
            var quantity = DataGenerator.RandomInt(1, 10);
            var volume = DataGenerator.RandomDecimal(25, 250);
            _ = _factory.BeverageConsumptionMeasurements.AddAsync(_person.Id, _alcoholicBeverage.Id, date, quantity, volume, _alcoholicBeverage.TypicalABV);
            var totals = await _factory.BeverageConsumptionCalculator.DailyTotalAlcoholAsync(_person.Id, date.AddDays(-1), date.AddDays(1));

            Assert.AreEqual(1, totals.Count);
            Assert.AreEqual(_person.Id, totals.First().PersonId);
            Assert.AreEqual(_alcoholicBeverage.Id, totals.First().BeverageId);
            Assert.AreEqual(date, totals.First().Date);
            Assert.AreEqual(_alcoholicBeverage.TypicalABV, totals.First().ABV);
            Assert.AreEqual(volume, totals.First().Volume);
            Assert.AreEqual(quantity, totals.First().Quantity);
        }

        [TestMethod]
        public async Task TotalAlcoholAsyncTest()
        {
            var date = DataGenerator.RandomDateInYear(2024);
            var from = date.AddDays(-1);
            var to = date.AddDays(1);
            var quantity = DataGenerator.RandomInt(1, 10);
            var volume = DataGenerator.RandomDecimal(25, 250);
            var expectedUnits = _factory.AlcoholUnitsCalculator.CalculateUnits(_alcoholicBeverage.TypicalABV, volume * quantity);

            _ = _factory.BeverageConsumptionMeasurements.AddAsync(_person.Id, _alcoholicBeverage.Id, date, quantity, volume, _alcoholicBeverage.TypicalABV);
            var total = await _factory.BeverageConsumptionCalculator.TotalAlcoholAsync(_person.Id, from, to);

            Assert.AreEqual(_person.Id, total.PersonId);
            Assert.AreEqual(_person.Name, total.PersonName);
            Assert.AreEqual(from, total.From);
            Assert.AreEqual(to, total.To);
            Assert.AreEqual(_alcoholicBeverage.Id, total.BeverageId);
            Assert.AreEqual(_alcoholicBeverage.Name, total.BeverageName);
            Assert.AreEqual(quantity * volume, total.TotalVolume);
            Assert.AreEqual(expectedUnits, total.TotalUnits);
        }
    }
}

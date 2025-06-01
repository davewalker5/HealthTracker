using HealthTracker.Configuration.Entities;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.Tests.Alcohol
{
    [TestClass]
    public class AlcoholUnitsCalculatorTest
    {
        private IHealthTrackerFactory _factory;

        [TestInitialize]
        public void Initialise()
        {
            _factory = new HealthTrackerFactory(null, null, null);
        }

        [TestMethod]
        public void UnitsTest()
        {
            var abv = DataGenerator.RandomDecimal(12, 40);
            var volume = DataGenerator.RandomDecimal(10, 250);
            var actual = _factory.AlcoholUnitsCalculator.CalculateUnits(abv, volume);
            var expected = Math.Round(abv * volume / 1000M, 2, MidpointRounding.AwayFromZero);
            Assert.AreEqual(expected, actual);
        }
    }
}
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
        private IHealthTrackerApplicationSettings _settings;
        private IHealthTrackerFactory _factory;

        [TestInitialize]
        public void Initialise()
        {
            _settings = new HealthTrackerApplicationSettings()
            {
                ShotSize = DataGenerator.RandomDecimal(25, 35),
                SmallGlassSize = 125M + DataGenerator.RandomDecimal(0, 75),
                MediumGlassSize = 175M + DataGenerator.RandomDecimal(0, 75),
                LargeGlassSize = 250M + DataGenerator.RandomDecimal(0, 75)
            };

            _factory = new HealthTrackerFactory(null, _settings, null);
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

        [TestMethod]
        public void ShotTest()
        {
            var abv = DataGenerator.RandomDecimal(12, 40);
            var actual = _factory.AlcoholUnitsCalculator.UnitsPerShot(abv);
            var expected = Math.Round(abv * _settings.ShotSize / 1000M, 2, MidpointRounding.AwayFromZero);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PintTest()
        {
            var abv = DataGenerator.RandomDecimal(3, 8);
            var actual = _factory.AlcoholUnitsCalculator.UnitsPerPint(abv);
            var expected = Math.Round(abv * 568M / 1000M, 2, MidpointRounding.AwayFromZero);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void HalfPintTest()
        {
            var abv = DataGenerator.RandomDecimal(3, 8);
            var actual = _factory.AlcoholUnitsCalculator.UnitsPerHalfPint(abv);
            var expected = Math.Round(abv * 284M / 1000M, 2, MidpointRounding.AwayFromZero);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SmallGlassTest()
        {
            var abv = DataGenerator.RandomDecimal(10, 15);
            var actual = _factory.AlcoholUnitsCalculator.UnitsPerSmallGlass(abv);
            var expected = Math.Round(abv * _settings.SmallGlassSize / 1000M, 2, MidpointRounding.AwayFromZero);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void MediumGlassTest()
        {
            var abv = DataGenerator.RandomDecimal(10, 15);
            var actual = _factory.AlcoholUnitsCalculator.UnitsPerMediumGlass(abv);
            var expected = Math.Round(abv * _settings.MediumGlassSize / 1000M, 2, MidpointRounding.AwayFromZero);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void LargeGlassTest()
        {
            var abv = DataGenerator.RandomDecimal(10, 15);
            var actual = _factory.AlcoholUnitsCalculator.UnitsPerLargeGlass(abv);
            var expected = Math.Round(abv * _settings.LargeGlassSize / 1000M, 2, MidpointRounding.AwayFromZero);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void LargeGlassVolumeTest()
        {
            var quantity = DataGenerator.RandomInt(1, 10);
            var actual = _factory.AlcoholUnitsCalculator.CalculateVolume(BeverageMeasure.LargeGlass, quantity);
            var expected = quantity * _settings.LargeGlassSize;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void MediumGlassVolumeTest()
        {
            var quantity = DataGenerator.RandomInt(1, 10);
            var actual = _factory.AlcoholUnitsCalculator.CalculateVolume(BeverageMeasure.MediumGlass, quantity);
            var expected = quantity * _settings.MediumGlassSize;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SmallGlassVolumeTest()
        {
            var quantity = DataGenerator.RandomInt(1, 10);
            var actual = _factory.AlcoholUnitsCalculator.CalculateVolume(BeverageMeasure.SmallGlass, quantity);
            var expected = quantity * _settings.SmallGlassSize;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ShotVolumeTest()
        {
            var quantity = DataGenerator.RandomInt(1, 10);
            var actual = _factory.AlcoholUnitsCalculator.CalculateVolume(BeverageMeasure.Shot, quantity);
            var expected = quantity * _settings.ShotSize;
            Assert.AreEqual(expected, actual);
        }
    }
}
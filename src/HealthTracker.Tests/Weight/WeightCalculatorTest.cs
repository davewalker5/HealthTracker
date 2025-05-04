using HealthTracker.Data;
using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using HealthTracker.Enumerations.Enumerations;
using Moq;

namespace HealthTracker.Tests.Weight
{
    [TestClass]
    public class WeightCalculatorTest
    {
        private readonly List<BMIBand> _bands = [
            new() { Name = "Underweight", MinimumBMI = 0, MaximumBMI = 18.49M, Order = 1 },
            new() { Name = "Normal", MinimumBMI = 18.5M, MaximumBMI = 24.9M, Order = 2 },
            new() { Name = "Overweight", MinimumBMI = 25M, MaximumBMI = 29.9M, Order = 3 },
            new() { Name = "Obese", MinimumBMI = 30M, MaximumBMI = 34.9M, Order = 4 },
            new() { Name = "Severely Obese", MinimumBMI = 35M, MaximumBMI = 39.9M, Order = 5 },
            new() { Name = "Morbidly Obese", MinimumBMI = 40M, MaximumBMI = 1000M, Order = 6 }
        ];

        private readonly Person _person = DataGenerator.RandomPerson(16, 90);
        private IHealthTrackerFactory _factory;
        private HealthTrackerDbContext _context;

        [TestInitialize]
        public async Task TestInitialize()
        {
            _context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            foreach (var band in _bands)
            {
                await _context.BMIBands.AddAsync(band);
            }
            await _context.People.AddAsync(_person);
            await _context.SaveChangesAsync();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(_context, null, logger.Object);
            Console.WriteLine(_person.Height);
        }

        [TestMethod]
        public async Task CalculateAverageTest()
        {
            var first = DataGenerator.RandomWeightMeasurement(_bands, "Normal", _person.Id, 2024, _person.Height);
            var second = DataGenerator.RandomWeightMeasurement(_bands, "Normal", _person.Id, 2024, _person.Height);

            await _factory.WeightMeasurements.AddAsync(_person.Id, first.Date, first.Weight);
            await _factory.WeightMeasurements.AddAsync(_person.Id, second.Date, second.Weight);

            var from = first.Date > second.Date ? second.Date : first.Date;
            var to = first.Date > second.Date ? first.Date : second.Date;
            var average = await _factory.WeightCalculator.AverageAsync(_person.Id, from, to);

            var expectedAverage = Math.Round((first.Weight + second.Weight)/ 2M, 2, MidpointRounding.AwayFromZero);
            Assert.AreEqual(_person.Id, average.PersonId);
            Assert.AreEqual(to, average.Date);
            Assert.AreEqual(expectedAverage, average.Weight);
        }

        [TestMethod]
        public async Task CalculateAverageWithNoMeasurementsTest()
        {
            var from = DateTime.Now.AddDays(-DataGenerator.RandomInt(30, 90));
            var to = DateTime.Now.AddDays(DataGenerator.RandomInt(30, 90));
            var average = await _factory.WeightCalculator.AverageAsync(_person.Id, from, to);
            Assert.IsNull(average);
        }

        [TestMethod]
        public async Task CalculateUnderweightBMITest()
        {
            var measurement = DataGenerator.RandomWeightMeasurement(_bands, "Underweight", _person.Id, 2024, _person.Height);
            var expectedBMI = measurement.BMI;
            var expectedBMR = ExpectedBMR(measurement);
            await _factory.WeightCalculator.CalculateRelatedProperties([measurement]);
            Assert.AreEqual(expectedBMI, measurement.BMI);
            Assert.AreEqual("Underweight", measurement.BMIAssessment);
            Assert.AreEqual(expectedBMR, measurement.BMR);
        }

        [TestMethod]
        public async Task CalculateNormalWeightBMITest()
        {
            var measurement = DataGenerator.RandomWeightMeasurement(_bands, "Normal", _person.Id, 2024, _person.Height);
            var expectedBMI = measurement.BMI;
            var expectedBMR = ExpectedBMR(measurement);
            await _factory.WeightCalculator.CalculateRelatedProperties([measurement]);
            Assert.AreEqual(expectedBMI, measurement.BMI);
            Assert.AreEqual("Normal", measurement.BMIAssessment);
            Assert.AreEqual(expectedBMR, measurement.BMR);
        }

        [TestMethod]
        public async Task CalculateOverweightBMITest()
        {
            var measurement = DataGenerator.RandomWeightMeasurement(_bands, "Overweight", _person.Id, 2024, _person.Height);
            var expectedBMI = measurement.BMI;
            var expectedBMR = ExpectedBMR(measurement);
            await _factory.WeightCalculator.CalculateRelatedProperties([measurement]);
            Assert.AreEqual(expectedBMI, measurement.BMI);
            Assert.AreEqual("Overweight", measurement.BMIAssessment);
            Assert.AreEqual(expectedBMR, measurement.BMR);
        }

        [TestMethod]
        public async Task CalculateObeseBMITest()
        {
            var measurement = DataGenerator.RandomWeightMeasurement(_bands, "Obese", _person.Id, 2024, _person.Height);
            var expectedBMI = measurement.BMI;
            var expectedBMR = ExpectedBMR(measurement);
            await _factory.WeightCalculator.CalculateRelatedProperties([measurement]);
            Assert.AreEqual(expectedBMI, measurement.BMI);
            Assert.AreEqual("Obese", measurement.BMIAssessment);
            Assert.AreEqual(expectedBMR, measurement.BMR);
        }

        [TestMethod]
        public async Task CalculateSeverelyObeseBMITest()
        {
            var measurement = DataGenerator.RandomWeightMeasurement(_bands, "Severely Obese", _person.Id, 2024, _person.Height);
            var expectedBMI = measurement.BMI;
            var expectedBMR = ExpectedBMR(measurement);
            await _factory.WeightCalculator.CalculateRelatedProperties([measurement]);
            Assert.AreEqual(expectedBMI, measurement.BMI);
            Assert.AreEqual("Severely Obese", measurement.BMIAssessment);
            Assert.AreEqual(expectedBMR, measurement.BMR);
        }

        [TestMethod]
        public async Task CalculateMorbidlyObeseBMITest()
        {
            var measurement = DataGenerator.RandomWeightMeasurement(_bands, "Morbidly Obese", _person.Id, 2024, _person.Height);
            var expectedBMI = measurement.BMI;
            var expectedBMR = ExpectedBMR(measurement);
            await _factory.WeightCalculator.CalculateRelatedProperties([measurement]);
            Assert.AreEqual(expectedBMI, measurement.BMI);
            Assert.AreEqual("Morbidly Obese", measurement.BMIAssessment);
            Assert.AreEqual(expectedBMR, measurement.BMR);
        }

        /// <summary>
        /// Calculate an expected BMR
        /// </summary>
        /// <param name="measurement"></param>
        /// <returns></returns>
        private decimal ExpectedBMR(WeightMeasurement measurement)
            =>  _person.Gender switch
                {
                    Gender.Male => (13.7516M * measurement.Weight) + (5.0033M * _person.Height * 100M) - (6.755M * _person.AgeInYears()) + 66.473M,
                    Gender.Female => (9.5634M * measurement.Weight) + (1.8496M * _person.Height * 100M) - (4.6756M * _person.AgeInYears()) + 655.0955M,
                    _ => 0M
                };
    }
}
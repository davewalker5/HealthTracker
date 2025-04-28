using HealthTracker.Data;
using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.BloodOxygenSaturation
{
    [TestClass]
    public class BloodOxygenSaturationCalculatorTest
    {
        private DateTime _firstMeasurementDate;
        private readonly decimal _firstSPO2 = DataGenerator.RandomSPO2Value();

        private DateTime _secondMeasurementDate;
        private readonly decimal _secondSPO2 = DataGenerator.RandomSPO2Value();

        private IHealthTrackerFactory _factory;
        private int _personId;

        [TestInitialize]
        public async Task TestInitialize()
        {
            _firstMeasurementDate = DataGenerator.RandomDateInYear(2024);
            _secondMeasurementDate = _firstMeasurementDate.AddDays(DataGenerator.RandomInt(1, 30));

            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            var person = await _factory.People.AddAsync("", "", DateTime.Now, 0, Gender.Unspecified);
            _personId = person.Id;
        }

        [TestMethod]
        public async Task CalculateAverageTest()
        {
            await _factory.BloodOxygenSaturationMeasurements.AddAsync(_personId, _firstMeasurementDate, _firstSPO2);
            await _factory.BloodOxygenSaturationMeasurements.AddAsync(_personId, _secondMeasurementDate, _secondSPO2);

            var average = await _factory.BloodOxygenSaturationCalculator.AverageAsync(_personId, _firstMeasurementDate, _secondMeasurementDate);
            var expected = Math.Round((_firstSPO2 + _secondSPO2) / 2.0M, 2, MidpointRounding.AwayFromZero);
            Assert.AreEqual(_personId, average.PersonId);
            Assert.AreEqual(_secondMeasurementDate, average.Date);
            Assert.AreEqual(expected, average.Percentage);
        }

        [TestMethod]
        public async Task CalculateAverageWithNoMeasurementsTest()
        {
            var average = await _factory.BloodOxygenSaturationCalculator.AverageAsync(_personId, _firstMeasurementDate, _secondMeasurementDate);
            Assert.IsNull(average);
        }

        [TestMethod]
        public async Task CalculateDailyAverageTest()
        {
            await _factory.BloodOxygenSaturationMeasurements.AddAsync(_personId, _firstMeasurementDate, _firstSPO2);
            await _factory.BloodOxygenSaturationMeasurements.AddAsync(_personId, _secondMeasurementDate, _secondSPO2);

            var averages = await _factory.BloodOxygenSaturationCalculator.DailyAverageAsync(_personId, _firstMeasurementDate, _secondMeasurementDate);
            Assert.AreEqual(2, averages.Count);

            Assert.AreEqual(_personId, averages[0].PersonId);
            Assert.AreEqual(_firstMeasurementDate, averages[0].Date);
            Assert.AreEqual(_firstSPO2, averages[0].Percentage);

            Assert.AreEqual(_personId, averages[1].PersonId);
            Assert.AreEqual(_secondMeasurementDate, averages[1].Date);
            Assert.AreEqual(_secondSPO2, averages[1].Percentage);
        }
    }
}

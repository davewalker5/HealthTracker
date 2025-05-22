using HealthTracker.Data;
using HealthTracker.Enumerations.Enumerations;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.BloodPressure
{
    [TestClass]
    public class BloodPressureCalculatorTest
    {
        private BloodPressureMeasurement _firstMeasurement;
        private BloodPressureMeasurement _secondMeasurement;
        private IHealthTrackerFactory _factory;
        private int _personId;

        [TestInitialize]
        public void TestInitialize()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            _personId = Task.Run(() => _factory.People.AddAsync("", "", DateTime.Now, 0, Gender.Unspecified)).Result.Id;
            _firstMeasurement = DataGenerator.RandomBloodPressureMeasurement(_personId, 2024, 0, 119, 0, 79);
            _secondMeasurement = DataGenerator.RandomBloodPressureMeasurement(_personId, 2025, 0, 119, 0, 79);
        }

        [TestMethod]
        public async Task CalculateAverageTest()
        {
            await _factory.BloodPressureMeasurements.AddAsync(_personId, _firstMeasurement.Date, _firstMeasurement.Systolic, _firstMeasurement.Diastolic);
            await _factory.BloodPressureMeasurements.AddAsync(_personId, _secondMeasurement.Date, _secondMeasurement.Systolic, _secondMeasurement.Diastolic);

            var average = await _factory.BloodPressureCalculator.AverageAsync(_personId, _firstMeasurement.Date, _secondMeasurement.Date);
            List<BloodPressureMeasurement> measurements = [_firstMeasurement, _secondMeasurement];
            var expectedAverageSystolic = (int)Math.Round(measurements.Select(x => x.Systolic).Average(), 0, MidpointRounding.AwayFromZero);
            var expectedAverageDiastolic = (int)Math.Round(measurements.Select(x => x.Diastolic).Average(), 0, MidpointRounding.AwayFromZero);
            Assert.AreEqual(_personId, average.PersonId);
            Assert.AreEqual(_secondMeasurement.Date, average.Date);
            Assert.AreEqual(expectedAverageSystolic, average.Systolic);
            Assert.AreEqual(expectedAverageDiastolic, average.Diastolic);
        }

        [TestMethod]
        public async Task CalculateAverageWithNoMeasurementsTest()
        {
            var average = await _factory.BloodPressureCalculator.AverageAsync(_personId, _firstMeasurement.Date, _secondMeasurement.Date);
            Assert.IsNull(average);
        }

        [TestMethod]
        public async Task CalculateDailyAverageTest()
        {
            await _factory.BloodPressureMeasurements.AddAsync(_personId, _firstMeasurement.Date, _firstMeasurement.Systolic, _firstMeasurement.Diastolic);
            await _factory.BloodPressureMeasurements.AddAsync(_personId, _secondMeasurement.Date, _secondMeasurement.Systolic, _secondMeasurement.Diastolic);

            var averages = await _factory.BloodPressureCalculator.DailyAverageAsync(_personId, _firstMeasurement.Date, _secondMeasurement.Date);
            Assert.AreEqual(2, averages.Count);

            Assert.AreEqual(_personId, averages[0].PersonId);
            Assert.AreEqual(_firstMeasurement.Date, averages[0].Date);
            Assert.AreEqual(_firstMeasurement.Systolic, averages[0].Systolic);
            Assert.AreEqual(_firstMeasurement.Diastolic, averages[0].Diastolic);

            Assert.AreEqual(_personId, averages[1].PersonId);
            Assert.AreEqual(_secondMeasurement.Date, averages[1].Date);
            Assert.AreEqual(_secondMeasurement.Systolic, averages[1].Systolic);
            Assert.AreEqual(_secondMeasurement.Diastolic, averages[1].Diastolic);
        }
    }
}

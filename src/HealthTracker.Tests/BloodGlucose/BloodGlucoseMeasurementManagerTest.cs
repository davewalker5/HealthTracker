using HealthTracker.Data;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Enumerations.Enumerations;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.BloodGlucose
{
    [TestClass]
    public class BloodGlucoseMeasurementManagerTest
    {
        private readonly DateTime _measurementDate = DataGenerator.RandomDateInYear(2024);
        private readonly decimal _level = DataGenerator.RandomBloodGlucoseValue();

        private readonly DateTime _UpdatedMeasurementDate = DataGenerator.RandomDateInYear(2024);
        private readonly decimal _updatedLevel = DataGenerator.RandomBloodGlucoseValue();

        private IHealthTrackerFactory _factory;
        private int _personId;
        private int _measurementId;

        [TestInitialize]
        public void TestInitialize()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            _personId = Task.Run(() => _factory.People.AddAsync("", "", DateTime.Now, 0, Gender.Unspecified)).Result.Id;
            _measurementId = Task.Run(() => _factory.BloodGlucoseMeasurements.AddAsync(_personId, _measurementDate, _level)).Result.Id;
        }

        [TestMethod]
        public async Task AddAndListTest()
        {
            var measurements = await _factory.BloodGlucoseMeasurements.ListAsync(x => x.PersonId == _personId);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(_measurementId, measurements.First().Id);
            Assert.AreEqual(_personId, measurements.First().PersonId);
            Assert.AreEqual(_measurementDate, measurements.First().Date);
            Assert.AreEqual(_level, measurements.First().Level);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            await _factory.BloodGlucoseMeasurements.UpdateAsync(_measurementId, _personId, _UpdatedMeasurementDate, _updatedLevel);
            var measurements = await _factory.BloodGlucoseMeasurements.ListAsync(x => x.PersonId == _personId);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(_measurementId, measurements.First().Id);
            Assert.AreEqual(_personId, measurements.First().PersonId);
            Assert.AreEqual(_UpdatedMeasurementDate, measurements.First().Date);
            Assert.AreEqual(_updatedLevel, measurements.First().Level);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            await _factory.BloodGlucoseMeasurements.DeleteAsync(_measurementId);
            var measurements = await _factory.BloodGlucoseMeasurements.ListAsync(a => true);
            Assert.AreEqual(0, measurements.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(PersonNotFoundException))]
        public async Task CannotAddMeasurementForMissingPersonTest()
            => await _factory.BloodGlucoseMeasurements.AddAsync(10 * _personId, _measurementDate, _level);

        [TestMethod]
        [ExpectedException(typeof(PersonNotFoundException))]
        public async Task CannotUpdateMeasurementForMissingPersonTest()
            => await _factory.BloodGlucoseMeasurements.UpdateAsync(_measurementId, 10 * _personId, _UpdatedMeasurementDate, _updatedLevel);
    }
}

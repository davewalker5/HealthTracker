using HealthTracker.Data;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Enumerations.Enumerations;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.BloodOxygenSaturation
{
    [TestClass]
    public class BloodOxygenSaturationMeasurementManagerTest
    {
        private readonly DateTime _measurementDate = DataGenerator.RandomDateInYear(2024);
        private readonly decimal _SPO2 = DataGenerator.RandomSPO2Value();

        private readonly DateTime _UpdatedMeasurementDate = DataGenerator.RandomDateInYear(2024);
        private readonly decimal _UpdatedSPO2 = DataGenerator.RandomSPO2Value();

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
            _measurementId = Task.Run(() => _factory.BloodOxygenSaturationMeasurements.AddAsync(_personId, _measurementDate, _SPO2)).Result.Id;
        }

        [TestMethod]
        public async Task AddAndListTest()
        {
            var measurements = await _factory.BloodOxygenSaturationMeasurements.ListAsync(x => x.PersonId == _personId, 1, int.MaxValue);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(_measurementId, measurements.First().Id);
            Assert.AreEqual(_personId, measurements.First().PersonId);
            Assert.AreEqual(_measurementDate, measurements.First().Date);
            Assert.AreEqual(_SPO2, measurements.First().Percentage);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            await _factory.BloodOxygenSaturationMeasurements.UpdateAsync(_measurementId, _personId, _UpdatedMeasurementDate, _UpdatedSPO2);
            var measurements = await _factory.BloodOxygenSaturationMeasurements.ListAsync(x => x.PersonId == _personId, 1, int.MaxValue);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(_measurementId, measurements.First().Id);
            Assert.AreEqual(_personId, measurements.First().PersonId);
            Assert.AreEqual(_UpdatedMeasurementDate, measurements.First().Date);
            Assert.AreEqual(_UpdatedSPO2, measurements.First().Percentage);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            await _factory.BloodOxygenSaturationMeasurements.DeleteAsync(_measurementId);
            var measurements = await _factory.BloodOxygenSaturationMeasurements.ListAsync(a => true, 1, int.MaxValue);
            Assert.AreEqual(0, measurements.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(PersonNotFoundException))]
        public async Task CannotAddMeasurementForMissingPersonTest()
            => await _factory.BloodOxygenSaturationMeasurements.AddAsync(10 * _personId, _measurementDate, _SPO2);

        [TestMethod]
        [ExpectedException(typeof(PersonNotFoundException))]
        public async Task CannotUpdateMeasurementForMissingPersonTest()
            => await _factory.BloodOxygenSaturationMeasurements.UpdateAsync(_measurementId, 10 * _personId, _UpdatedMeasurementDate, _UpdatedSPO2);
    }
}

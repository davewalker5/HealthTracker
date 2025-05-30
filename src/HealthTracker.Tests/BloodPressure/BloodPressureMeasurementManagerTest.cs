using HealthTracker.Data;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Enumerations.Enumerations;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.BloodPressure
{
    [TestClass]
    public class BloodPressureMeasurementManagerTest
    {
        private readonly DateTime MeasurementDate = DataGenerator.RandomDateInYear(2024);
        private readonly int Systolic = DataGenerator.RandomInt(0, 119);

        private readonly int Diastolic = DataGenerator.RandomInt(0, 79);

        private readonly DateTime UpdatedMeasurementDate = DataGenerator.RandomDateInYear(2024);
        private readonly int UpdatedSystolic = DataGenerator.RandomInt(0, 119);
        private readonly int UpdatedDiastolic = DataGenerator.RandomInt(0, 79);

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
            _measurementId = Task.Run(() => _factory.BloodPressureMeasurements.AddAsync(_personId, MeasurementDate, Systolic, Diastolic)).Result.Id;
        }

        [TestMethod]
        public async Task AddAndListTest()
        {
            var measurements = await _factory.BloodPressureMeasurements.ListAsync(x => x.PersonId == _personId, 1, int.MaxValue);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(_measurementId, measurements.First().Id);
            Assert.AreEqual(_personId, measurements.First().PersonId);
            Assert.AreEqual(MeasurementDate, measurements.First().Date);
            Assert.AreEqual(Systolic, measurements.First().Systolic);
            Assert.AreEqual(Diastolic, measurements.First().Diastolic);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            await _factory.BloodPressureMeasurements.UpdateAsync(_measurementId, _personId, UpdatedMeasurementDate, UpdatedSystolic, UpdatedDiastolic);
            var measurements = await _factory.BloodPressureMeasurements.ListAsync(x => x.PersonId == _personId, 1, int.MaxValue);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(_measurementId, measurements.First().Id);
            Assert.AreEqual(_personId, measurements.First().PersonId);
            Assert.AreEqual(UpdatedMeasurementDate, measurements.First().Date);
            Assert.AreEqual(UpdatedSystolic, measurements.First().Systolic);
            Assert.AreEqual(UpdatedDiastolic, measurements.First().Diastolic);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            await _factory.BloodPressureMeasurements.DeleteAsync(_measurementId);
            var measurements = await _factory.BloodPressureMeasurements.ListAsync(a => true, 1, int.MaxValue);
            Assert.AreEqual(0, measurements.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(PersonNotFoundException))]
        public async Task CannotAddMeasurementForMissingPersonTest()
            => await _factory.BloodPressureMeasurements.AddAsync(10 * _personId, MeasurementDate, Systolic, Diastolic);

        [TestMethod]
        [ExpectedException(typeof(PersonNotFoundException))]
        public async Task CannotUpdateMeasurementForMissingPersonTest()
            => await _factory.BloodPressureMeasurements.UpdateAsync(_measurementId, 10 * _personId, UpdatedMeasurementDate, UpdatedSystolic, UpdatedDiastolic);
    }
}

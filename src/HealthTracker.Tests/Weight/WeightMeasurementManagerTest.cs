using HealthTracker.Data;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Enumerations.Enumerations;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.Weight
{
    [TestClass]
    public class WeightMeasurementManagerTest
    {
        private readonly DateTime MeasurementDate = DataGenerator.RandomDateInYear(2024);
        private readonly decimal Weight = DataGenerator.RandomDecimal(50, 100);

        private readonly DateTime UpdatedMeasurementDate = DataGenerator.RandomDateInYear(2025);
        private readonly decimal UpdatedWeight = DataGenerator.RandomDecimal(50, 100);

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
            _measurementId = Task.Run(() => _factory.WeightMeasurements.AddAsync(_personId, MeasurementDate, Weight)).Result.Id;
        }

        [TestMethod]
        public async Task AddAndListTest()
        {
            var measurements = await _factory.WeightMeasurements.ListAsync(x => x.PersonId == _personId);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(_measurementId, measurements.First().Id);
            Assert.AreEqual(_personId, measurements.First().PersonId);
            Assert.AreEqual(MeasurementDate, measurements.First().Date);
            Assert.AreEqual(Weight, measurements.First().Weight);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            await _factory.WeightMeasurements.UpdateAsync(_measurementId, _personId, UpdatedMeasurementDate, UpdatedWeight);
            var measurements = await _factory.WeightMeasurements.ListAsync(x => x.PersonId == _personId);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(_measurementId, measurements.First().Id);
            Assert.AreEqual(_personId, measurements.First().PersonId);
            Assert.AreEqual(UpdatedMeasurementDate, measurements.First().Date);
            Assert.AreEqual(UpdatedWeight, measurements.First().Weight);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            await _factory.WeightMeasurements.DeleteAsync(_measurementId);
            var measurements = await _factory.WeightMeasurements.ListAsync(a => true);
            Assert.AreEqual(0, measurements.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(PersonNotFoundException))]
        public async Task CannotAddMeasurementForMissingPersonTest()
            => await _factory.WeightMeasurements.AddAsync(10 * _personId, MeasurementDate, Weight);

        [TestMethod]
        [ExpectedException(typeof(PersonNotFoundException))]
        public async Task CannotUpdateMeasurementForMissingPersonTest()
            => await _factory.WeightMeasurements.UpdateAsync(_measurementId, 10 * _personId, UpdatedMeasurementDate, UpdatedWeight);
    }
}
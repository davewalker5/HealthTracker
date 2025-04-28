using HealthTracker.Data;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.Cholesterol
{
    [TestClass]
    public class CholesterolMeasurementManagerTest
    {
        private readonly DateTime MeasurementDate = DataGenerator.RandomDateInYear(2024);
        private readonly decimal Total = DataGenerator.RandomTotalLipids();
        private readonly decimal HDL = DataGenerator.RandomHDL();
        private readonly decimal LDL = DataGenerator.RandomLDL();
        private readonly decimal Triglycerides = DataGenerator.RandomTriglycerides();

        private readonly DateTime UpdatedMeasurementDate = DataGenerator.RandomDateInYear(2025);
        private readonly decimal UpdatedTotal = DataGenerator.RandomTotalLipids();
        private readonly decimal UpdatedHDL = DataGenerator.RandomHDL();
        private readonly decimal UpdatedLDL = DataGenerator.RandomLDL();
        private readonly decimal UpdatedTriglycerides = DataGenerator.RandomTriglycerides();

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
            _measurementId = Task.Run(() => _factory.CholesterolMeasurements.AddAsync(_personId, MeasurementDate, Total, HDL, LDL, Triglycerides)).Result.Id;
        }

        [TestMethod]
        public async Task AddAndListTest()
        {
            var measurements = await _factory.CholesterolMeasurements.ListAsync(x => x.PersonId == _personId);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(_measurementId, measurements.First().Id);
            Assert.AreEqual(_personId, measurements.First().PersonId);
            Assert.AreEqual(MeasurementDate, measurements.First().Date);
            Assert.AreEqual(Total, measurements.First().Total);
            Assert.AreEqual(HDL, measurements.First().HDL);
            Assert.AreEqual(LDL, measurements.First().LDL);
            Assert.AreEqual(Triglycerides, measurements.First().Triglycerides);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            await _factory.CholesterolMeasurements.UpdateAsync(_measurementId, _personId, UpdatedMeasurementDate, UpdatedTotal, UpdatedHDL, UpdatedLDL, UpdatedTriglycerides);
            var measurements = await _factory.CholesterolMeasurements.ListAsync(x => x.PersonId == _personId);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(_measurementId, measurements.First().Id);
            Assert.AreEqual(_personId, measurements.First().PersonId);
            Assert.AreEqual(UpdatedMeasurementDate, measurements.First().Date);
            Assert.AreEqual(UpdatedTotal, measurements.First().Total);
            Assert.AreEqual(UpdatedHDL, measurements.First().HDL);
            Assert.AreEqual(UpdatedLDL, measurements.First().LDL);
            Assert.AreEqual(UpdatedTriglycerides, measurements.First().Triglycerides);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            await _factory.CholesterolMeasurements.DeleteAsync(_measurementId);
            var measurements = await _factory.CholesterolMeasurements.ListAsync(a => true);
            Assert.AreEqual(0, measurements.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(PersonNotFoundException))]
        public async Task CannotAddMeasurementForMissingPersonTest()
            => await _factory.CholesterolMeasurements.AddAsync(10 * _personId, MeasurementDate, Total, HDL, LDL, Triglycerides);

        [TestMethod]
        [ExpectedException(typeof(PersonNotFoundException))]
        public async Task CannotUpdateMeasurementForMissingPersonTest()
            => await _factory.CholesterolMeasurements.UpdateAsync(_measurementId, 10 * _personId, UpdatedMeasurementDate, UpdatedTotal, UpdatedHDL, UpdatedLDL, UpdatedTriglycerides);
    }
}

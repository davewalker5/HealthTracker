using HealthTracker.Data;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Enumerations.Enumerations;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Logic.Extensions;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.Beverages
{
    [TestClass]
    public class BeverageConsumptionMeasurementManagerTest
    {
        private readonly DateTime ConsumptionDate = DataGenerator.RandomDateInYear(2024);
        private readonly TempBeverageMeasure Measure = DataGenerator.RandomTempBeverageMeasure();
        private readonly int Quantity = DataGenerator.RandomDuration().ToDuration();
        private readonly decimal Volume = DataGenerator.RandomDecimal(25, 250);
        private readonly decimal ABV = DataGenerator.RandomDecimal(0, 20);

        private readonly DateTime UpdatedConsumptionDate = DataGenerator.RandomDateInYear(2024);
        private readonly TempBeverageMeasure UpdatedMeasure = DataGenerator.RandomTempBeverageMeasure();
        private readonly int UpdatedQuantity = DataGenerator.RandomDuration().ToDuration();
        private readonly decimal UpdatedVolume = DataGenerator.RandomDecimal(25, 250);
        private readonly decimal UpdatedABV = DataGenerator.RandomDecimal(21, 40);

        private IHealthTrackerFactory _factory;
        private int _personId;
        private int _beverageId;
        private int _measurementId;

        [TestInitialize]
        public void TestInitialize()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            _personId = Task.Run(() => _factory.People.AddAsync("", "", DateTime.Now, 0, Gender.Unspecified)).Result.Id;
            _beverageId = Task.Run(() => _factory.Beverages.AddAsync("White Wine", DataGenerator.RandomDecimal(0, 13), false, false)).Result.Id;
            _measurementId = Task.Run(() => _factory.BeverageConsumptionMeasurements.AddAsync(_personId, _beverageId, ConsumptionDate, Measure, Quantity, Volume, ABV)).Result.Id;
        }

        [TestMethod]
        public async Task AddAndListTest()
        {
            var measurements = await _factory.BeverageConsumptionMeasurements.ListAsync(x => x.PersonId == _personId, 1, int.MaxValue);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(_measurementId, measurements.First().Id);
            Assert.AreEqual(_personId, measurements.First().PersonId);
            Assert.AreEqual(_beverageId, measurements.First().BeverageId);
            Assert.AreEqual(ConsumptionDate, measurements.First().Date);
            Assert.AreEqual(Measure, measurements.First().Measure);
            Assert.AreEqual(Quantity, measurements.First().Quantity);
            Assert.AreEqual(ABV, measurements.First().ABV);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            await _factory.BeverageConsumptionMeasurements.UpdateAsync(_measurementId, _personId, _beverageId, UpdatedConsumptionDate, UpdatedMeasure, UpdatedQuantity, UpdatedVolume, UpdatedABV);
            var measurements = await _factory.BeverageConsumptionMeasurements.ListAsync(x => x.PersonId == _personId, 1, int.MaxValue);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(_measurementId, measurements.First().Id);
            Assert.AreEqual(_personId, measurements.First().PersonId);
            Assert.AreEqual(_beverageId, measurements.First().BeverageId);
            Assert.AreEqual(UpdatedConsumptionDate, measurements.First().Date);
            Assert.AreEqual(UpdatedMeasure, measurements.First().Measure);
            Assert.AreEqual(UpdatedQuantity, measurements.First().Quantity);
            Assert.AreEqual(UpdatedABV, measurements.First().ABV);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            await _factory.BeverageConsumptionMeasurements.DeleteAsync(_measurementId);
            var measurements = await _factory.BeverageConsumptionMeasurements.ListAsync(a => true, 1, int.MaxValue);
            Assert.AreEqual(0, measurements.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(PersonNotFoundException))]
        public async Task CannotAddMeasurementForMissingPersonTest()
            => await _factory.BeverageConsumptionMeasurements.AddAsync(10 * _personId, _beverageId, ConsumptionDate, Measure, Quantity, Volume, ABV);

        [TestMethod]
        [ExpectedException(typeof(PersonNotFoundException))]
        public async Task CannotUpdateMeasurementForMissingPersonTest()
            => await _factory.BeverageConsumptionMeasurements.UpdateAsync(_measurementId, 10 * _personId, _beverageId, UpdatedConsumptionDate, UpdatedMeasure, UpdatedQuantity, UpdatedVolume, UpdatedABV);

        [TestMethod]
        [ExpectedException(typeof(BeverageNotFoundException))]
        public async Task CannotAddMeasurementForMissingBeverageTest()
            => await _factory.BeverageConsumptionMeasurements.AddAsync(_personId, 10 * _beverageId, ConsumptionDate, Measure, Quantity, Volume, ABV);

        [TestMethod]
        [ExpectedException(typeof(BeverageNotFoundException))]
        public async Task CannotUpdateMeasurementForMissingActivityTypeTest()
            => await _factory.BeverageConsumptionMeasurements.UpdateAsync(_measurementId, _personId, 10 * _beverageId, UpdatedConsumptionDate, UpdatedMeasure, UpdatedQuantity, UpdatedVolume, UpdatedABV);
    }
}

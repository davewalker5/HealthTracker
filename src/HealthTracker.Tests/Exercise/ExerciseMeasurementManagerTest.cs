using HealthTracker.Data;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Logic.Extensions;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.Exercise
{
    [TestClass]
    public class ExerciseMeasurementManagerTest
    {
        private readonly DateTime ExerciseDate = DataGenerator.RandomDateInYear(2023);
        private readonly int Duration = DataGenerator.RandomDuration().ToDuration();
        private readonly decimal? Distance = DataGenerator.RandomDecimal(10, 50);
        private readonly int Calories = DataGenerator.RandomInt(500, 3000);
        private readonly int MinimumHeartRate = DataGenerator.RandomInt(50, 80);
        private readonly int MaximumHeartRate = DataGenerator.RandomInt(120, 160);


        private readonly DateTime UpdatedExerciseDate = DataGenerator.RandomDateInYear(2023);
        private readonly int UpdatedDuration = DataGenerator.RandomDuration().ToDuration();
        private readonly decimal? UpdatedDistance = null;
        private readonly int UpdatedCalories = DataGenerator.RandomInt(500, 3000);
        private readonly int UpdatedMinimumHeartRate = DataGenerator.RandomInt(50, 80);
        private readonly int UpdatedMaximumHeartRate = DataGenerator.RandomInt(120, 160);

        private IHealthTrackerFactory _factory;
        private int _personId;
        private int _activityTypeId;
        private int _measurementId;

        [TestInitialize]
        public void TestInitialize()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            _personId = Task.Run(() => _factory.People.AddAsync("", "", DateTime.Now, 0, Gender.Unspecified)).Result.Id;
            _activityTypeId = Task.Run(() => _factory.ActivityTypes.AddAsync("Cycling")).Result.Id;
            _measurementId = Task.Run(() => _factory.ExerciseMeasurements.AddAsync(_personId, _activityTypeId, ExerciseDate, Duration, Distance, Calories, MinimumHeartRate, MaximumHeartRate)).Result.Id;
        }

        [TestMethod]
        public async Task AddAndListTest()
        {
            var measurements = await _factory.ExerciseMeasurements.ListAsync(x => x.PersonId == _personId);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(_measurementId, measurements.First().Id);
            Assert.AreEqual(_personId, measurements.First().PersonId);
            Assert.AreEqual(_activityTypeId, measurements.First().ActivityTypeId);
            Assert.AreEqual(ExerciseDate, measurements.First().Date);
            Assert.AreEqual(Duration, measurements.First().Duration);
            Assert.AreEqual(Distance, measurements.First().Distance);
            Assert.AreEqual(Calories, measurements.First().Calories);
            Assert.AreEqual(MinimumHeartRate, measurements.First().MinimumHeartRate);
            Assert.AreEqual(MaximumHeartRate, measurements.First().MaximumHeartRate);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            await _factory.ExerciseMeasurements.UpdateAsync(_measurementId, _personId, _activityTypeId, UpdatedExerciseDate, UpdatedDuration, UpdatedDistance, UpdatedCalories, UpdatedMinimumHeartRate, UpdatedMaximumHeartRate);
            var measurements = await _factory.ExerciseMeasurements.ListAsync(x => x.PersonId == _personId);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(_measurementId, measurements.First().Id);
            Assert.AreEqual(_personId, measurements.First().PersonId);
            Assert.AreEqual(_activityTypeId, measurements.First().ActivityTypeId);
            Assert.AreEqual(UpdatedExerciseDate, measurements.First().Date);
            Assert.AreEqual(UpdatedDuration, measurements.First().Duration);
            Assert.IsNull(measurements.First().Distance);
            Assert.AreEqual(UpdatedCalories, measurements.First().Calories);
            Assert.AreEqual(UpdatedMinimumHeartRate, measurements.First().MinimumHeartRate);
            Assert.AreEqual(UpdatedMaximumHeartRate, measurements.First().MaximumHeartRate);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            await _factory.ExerciseMeasurements.DeleteAsync(_measurementId);
            var measurements = await _factory.ExerciseMeasurements.ListAsync(a => true);
            Assert.AreEqual(0, measurements.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(PersonNotFoundException))]
        public async Task CannotAddMeasurementForMissingPersonTest()
            => await _factory.ExerciseMeasurements.AddAsync(10 * _personId, _activityTypeId, ExerciseDate, Duration, Distance, Calories, MinimumHeartRate, MaximumHeartRate);

        [TestMethod]
        [ExpectedException(typeof(PersonNotFoundException))]
        public async Task CannotUpdateMeasurementForMissingPersonTest()
            => await _factory.ExerciseMeasurements.UpdateAsync(_measurementId, 10 * _personId, _activityTypeId, UpdatedExerciseDate, UpdatedDuration, UpdatedDistance, UpdatedCalories, UpdatedMinimumHeartRate, UpdatedMaximumHeartRate);

        [TestMethod]
        [ExpectedException(typeof(ActivityTypeNotFoundException))]
        public async Task CannotAddMeasurementForMissingActivityTypeTest()
            => await _factory.ExerciseMeasurements.AddAsync(_personId, 10 * _activityTypeId, ExerciseDate, Duration, Distance, Calories, MinimumHeartRate, MaximumHeartRate);

        [TestMethod]
        [ExpectedException(typeof(ActivityTypeNotFoundException))]
        public async Task CannotUpdateMeasurementForMissingActivityTypeTest()
            => await _factory.ExerciseMeasurements.UpdateAsync(_measurementId, _personId, 10 * _activityTypeId, UpdatedExerciseDate, UpdatedDuration, UpdatedDistance, UpdatedCalories, UpdatedMinimumHeartRate, UpdatedMaximumHeartRate);
    }
}

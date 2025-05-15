using HealthTracker.Data;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.ActivityTypes
{
    [TestClass]
    public class ActivityTypeManagerTest
    {
        private readonly string Description = DataGenerator.RandomActivityTypeName();
        private readonly string UpdatedDescription = DataGenerator.RandomActivityTypeName();

        private IHealthTrackerFactory _factory;
        private int _activityTypeId;

        [TestInitialize]
        public void TestInitialize()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            _activityTypeId = Task.Run(() => _factory.ActivityTypes.AddAsync(Description)).Result.Id;
        }

        [TestMethod]
        public async Task AddAndGetTest()
        {
            var activityType = await _factory.ActivityTypes.GetAsync(a => a.Id == _activityTypeId);
            Assert.IsNotNull(activityType);
            Assert.AreEqual(_activityTypeId, activityType.Id);
            Assert.AreEqual(Description, activityType.Description);
        }

        [TestMethod]
        public async Task GetMissingTest()
        {
            var activityType = await _factory.ActivityTypes.GetAsync(a => a.Id == (10 * _activityTypeId));
            Assert.IsNull(activityType);
        }

        [TestMethod]
        public async Task ListAllTest()
        {
            var activityTypes = await _factory.ActivityTypes.ListAsync(x => true, 1, int.MaxValue);
            Assert.AreEqual(1, activityTypes.Count);
            Assert.AreEqual(Description, activityTypes.First().Description);
        }

        [TestMethod]
        public async Task ListMissingTest()
        {
            var activityTypes = await _factory.ActivityTypes.ListAsync(e => e.Description == "Missing", 1, int.MaxValue);
            Assert.AreEqual(0, activityTypes.Count);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            await _factory.ActivityTypes.UpdateAsync(_activityTypeId, UpdatedDescription);
            var activityType = await _factory.ActivityTypes.GetAsync(a => a.Id == _activityTypeId);
            Assert.IsNotNull(activityType);
            Assert.AreEqual(_activityTypeId, activityType.Id);
            Assert.AreEqual(UpdatedDescription, activityType.Description);
            Assert.AreNotEqual(Description, UpdatedDescription);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            await _factory.ActivityTypes.DeleteAsync(_activityTypeId);
            var activityTypes = await _factory.ActivityTypes.ListAsync(a => true, 1, int.MaxValue);
            Assert.AreEqual(0, activityTypes.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ActivityTypeExistsException))]
        public async Task CannotAddDuplicateActivityTypeTest()
            => _ = await _factory.ActivityTypes.AddAsync(Description);

        [TestMethod]
        [ExpectedException(typeof(ActivityTypeInUseException))]
        public async Task CannotDeleteWithExerciseMeasurementTest()
        {
            var context = _factory.Context as HealthTrackerDbContext;
            await context.ExerciseMeasurements.AddAsync(new ExerciseMeasurement
            {
                ActivityTypeId = _activityTypeId,
                Distance = DataGenerator.RandomDecimal(0M, 50M),
            });
            await context.SaveChangesAsync();
            await _factory.ActivityTypes.DeleteAsync(_activityTypeId);
        }
    }
}
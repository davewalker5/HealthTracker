using HealthTracker.Logic.Factory;
using HealthTracker.Data;
using Moq;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Tests.Mocks;

namespace HealthTracker.Tests.JobStatuses
{
    [TestClass]
    public class JobStatusManagerTest
    {
        private readonly string Name = DataGenerator.RandomWord(10, 20);
        private readonly string Parameters = $"{DataGenerator.RandomWord(10, 20)}.csv";
        private readonly string Error = DataGenerator.RandomPhrase(5, 5, 10);

        private IHealthTrackerFactory _factory;
        private long _statusId;

        [TestInitialize]
        public void TestInitialize()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            _statusId = Task.Run(() => _factory.JobStatuses.AddAsync(Name, Parameters)).Result.Id;
        }

        [TestMethod]
        public async Task AddAndGetAsyncTest()
        {
            var status = await _factory.JobStatuses.GetAsync(x => x.Id == _statusId);

            Assert.IsNotNull(status);
            Assert.AreEqual(_statusId, status.Id);
            Assert.AreEqual(Name, status.Name);
            Assert.AreEqual(Parameters, status.Parameters);
            Assert.IsNotNull(status.Start);
            Assert.IsNull(status.End);
            Assert.IsNull(status.Error);
        }

        [TestMethod]
        public async Task ListAsyncTest()
        {
            var statuses = await _factory.JobStatuses.ListAsync(x => true, 1, int.MaxValue);

            Assert.IsNotNull(statuses);
            Assert.AreEqual(1, statuses.Count);
            Assert.AreEqual(_statusId, statuses[0].Id);
            Assert.AreEqual(Name, statuses[0].Name);
            Assert.AreEqual(Parameters, statuses[0].Parameters);
            Assert.IsNotNull(statuses[0].Start);
            Assert.IsNull(statuses[0].End);
            Assert.IsNull(statuses[0].Error);
        }

        [TestMethod]
        public async Task UpdateAsyncTest()
        {
            var status = await _factory.JobStatuses.UpdateAsync(_statusId, Error);

            Assert.IsNotNull(status);
            Assert.AreEqual(_statusId, status.Id);
            Assert.AreEqual(Name, status.Name);
            Assert.AreEqual(Parameters, status.Parameters);
            Assert.IsNotNull(status.Start);
            Assert.IsNotNull(status.End);
            Assert.AreEqual(Error, status.Error);
        }
    }
}

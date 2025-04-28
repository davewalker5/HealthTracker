using HealthTracker.Configuration.Entities;
using HealthTracker.Data;
using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Medications;
using HealthTracker.Logic.Extensions;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.MedicationTracking
{
    [TestClass]
    public class MedicationActionGeneratorManagerTest
    {
        private readonly Person _person = DataGenerator.RandomPerson(16, 90);
        private readonly Medication _medication = DataGenerator.RandomMedication();
        private readonly int LeadTimeDays = DataGenerator.RandomInt(10, 20);
        private readonly DateTime Today = HealthTrackerDateExtensions.TodayWithoutTime();

        private IHealthTrackerFactory _factory;

        [TestInitialize]
        public async Task TestInitialize()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var settings = new HealthTrackerApplicationSettings{ MedicationOrderLeadTimeDays = LeadTimeDays };
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, settings, logger.Object);
            _person.Id = (await _factory.People.AddAsync(_person.FirstNames, _person.Surname, _person.DateOfBirth, _person.Height, _person.Gender)).Id;
            _medication.Id = Task.Run(() => _factory.Medications.AddAsync(_medication.Name)).Result.Id;
        }

        [TestMethod]
        public async Task NoActionsTest()
        {
            var association = await _factory.PersonMedications.AddAsync(_person.Id, _medication.Id, 1, 2 * LeadTimeDays, Today);
            Assert.IsNull(association.Actions);
            _factory.MedicationActionGenerator.DetermineActions(association);
            Assert.IsNotNull(association.Actions);
            Assert.AreEqual(0, association.Actions.Count);
        }

        [TestMethod]
        public async Task TakeDoseTest()
        {
            var association = await _factory.PersonMedications.AddAsync(_person.Id, _medication.Id, 1, 2 * LeadTimeDays, Today.AddDays(-1));
            Assert.IsNull(association.Actions);
            _factory.MedicationActionGenerator.DetermineActions(association);
            Assert.IsNotNull(association.Actions);
            Assert.AreEqual(1, association.Actions.Count);
            Assert.AreEqual("Take dose", association.Actions[0]);
        }

        [TestMethod]
        public async Task OrderMoreTest()
        {
            var association = await _factory.PersonMedications.AddAsync(_person.Id, _medication.Id, 1, LeadTimeDays, Today);
            Assert.IsNull(association.Actions);
            _factory.MedicationActionGenerator.DetermineActions(association);
            Assert.IsNotNull(association.Actions);
            Assert.AreEqual(1, association.Actions.Count);
            Assert.AreEqual("Order more", association.Actions[0]);
        }

        [TestMethod]
        public async Task MultipleActionTest()
        {
            var association = await _factory.PersonMedications.AddAsync(_person.Id, _medication.Id, 1, LeadTimeDays, Today.AddDays(-1));
            Assert.IsNull(association.Actions);
            _factory.MedicationActionGenerator.DetermineActions(association);
            Assert.IsNotNull(association.Actions);
            Assert.AreEqual(2, association.Actions.Count);
            Assert.IsTrue(association.Actions.Contains("Take dose"));
            Assert.IsTrue(association.Actions.Contains("Order more"));
        }

        [TestMethod]
        public async Task AssociationCollectionTest()
        {
            await _factory.PersonMedications.AddAsync(_person.Id, _medication.Id, 1, LeadTimeDays, Today.AddDays(-1));
            var associations = await _factory.PersonMedications.ListAsync(x => true);
            Assert.IsNull(associations[0].Actions);
            _factory.MedicationActionGenerator.DetermineActions(associations);
            Assert.IsNotNull(associations[0].Actions);
            Assert.AreEqual(2, associations[0].Actions.Count);
            Assert.IsTrue(associations[0].Actions.Contains("Take dose"));
            Assert.IsTrue(associations[0].Actions.Contains("Order more"));
        }

        [TestMethod]
        public async Task NoActionsForInactiveAssociationTest()
        {
            var association = await _factory.PersonMedications.AddAsync(_person.Id, _medication.Id, 1, LeadTimeDays, Today.AddDays(-1));
            await _factory.PersonMedications.DeactivateAsync(association.Id);
            _factory.MedicationActionGenerator.DetermineActions(association);
            Assert.IsNotNull(association.Actions);
            Assert.AreEqual(0, association.Actions.Count);
        }
    }
}
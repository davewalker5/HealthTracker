using HealthTracker.Data;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.Medications
{
    [TestClass]
    public class MedicationManagerTest
    {
        private readonly Person _person = DataGenerator.RandomPerson(16, 90);
        private readonly string Name = DataGenerator.RandomMedicationName();
        private readonly string UpdatedName = DataGenerator.RandomMedicationName();

        private IHealthTrackerFactory _factory;
        private int _medicationId;

        [TestInitialize]
        public void TestInitialize()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            _medicationId = Task.Run(() => _factory.Medications.AddAsync(Name)).Result.Id;
        }

        [TestMethod]
        public async Task AddAndGetTest()
        {
            var medication = await _factory.Medications.GetAsync(a => a.Id == _medicationId);
            Assert.IsNotNull(medication);
            Assert.AreEqual(_medicationId, medication.Id);
            Assert.AreEqual(Name, medication.Name);
        }
        
        [TestMethod]
        public async Task GetMissingTest()
        {
            var medication = await _factory.Medications.GetAsync(a => a.Id == (10 * _medicationId));
            Assert.IsNull(medication);
        }

        [TestMethod]
        public async Task ListAllTest()
        {
            var medications = await _factory.Medications.ListAsync(x => true);
            Assert.AreEqual(1, medications.Count);
            Assert.AreEqual(Name, medications.First().Name);
        }

        [TestMethod]
        public async Task ListMissingTest()
        {
            var medications = await _factory.Medications.ListAsync(e => e.Name == "Missing");
            Assert.AreEqual(0, medications.Count);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            await _factory.Medications.UpdateAsync(_medicationId, UpdatedName);
            var medication = await _factory.Medications.GetAsync(a => a.Id == _medicationId);
            Assert.IsNotNull(medication);
            Assert.AreEqual(_medicationId, medication.Id);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            await _factory.Medications.DeleteAsync(_medicationId);
            var medications = await _factory.People.ListAsync(a => true, 1, int.MaxValue);
            Assert.AreEqual(0, medications.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(DuplicateMedicationException))]
        public async Task CannotAddDuplicateTest()
            => await _factory.Medications.AddAsync(Name);

        [TestMethod]
        [ExpectedException(typeof(DuplicateMedicationException))]
        public async Task CannotUpdateToProduceDuplicateTest()
        {
            var medication = await _factory.Medications.AddAsync(UpdatedName);
            _ = await _factory.Medications.UpdateAsync(medication.Id, Name);
        }

        [TestMethod]
        [ExpectedException(typeof(MedicationInUseException))]
        public async Task CannotDeleteWithAssociationsTest()
        {
            var person = await _factory.People.AddAsync(_person.FirstNames, _person.Surname, _person.DateOfBirth, _person.Height, _person.Gender);
            await _factory.PersonMedications.AddAsync(person.Id, _medicationId, 1, 0, null);
            await _factory.Medications.DeleteAsync(_medicationId);
        }
    }
}
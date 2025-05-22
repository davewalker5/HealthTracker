using HealthTracker.Data;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.PersonMedicationAssociations
{
    [TestClass]
    public class PersonMedicationManagerTest
    {
        private readonly Person _person = DataGenerator.RandomPerson(16, 90);
        private readonly string FirstMedicationName = DataGenerator.RandomMedicationName();
        private readonly string SecondMedicationName = DataGenerator.RandomMedicationName();

        private IHealthTrackerFactory _factory;
        private int _personId;
        private int _firstMedicationId;
        private int _secondMedicationId;
        private int _associationId;

        [TestInitialize]
        public void TestInitialize()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            _personId = Task.Run(() => _factory.People.AddAsync(_person.FirstNames, _person.Surname, _person.DateOfBirth, _person.Height, _person.Gender)).Result.Id;
            _firstMedicationId = Task.Run(() => _factory.Medications.AddAsync(FirstMedicationName)).Result.Id;
            _secondMedicationId = Task.Run(() => _factory.Medications.AddAsync(SecondMedicationName)).Result.Id;
            _associationId = Task.Run(() => _factory.PersonMedications.AddAsync(_personId, _firstMedicationId, 1, 0, null)).Result.Id;
        }

        [TestMethod]
        public async Task ListAllTest()
        {
            var associations = await _factory.PersonMedications.ListAsync(x => true, 1, int.MaxValue);
            Assert.AreEqual(1, associations.Count);
            Assert.AreEqual(_personId, associations.First().PersonId);
            Assert.AreEqual(_firstMedicationId, associations.First().MedicationId);
            Assert.AreEqual(1, associations.First().DailyDose);
            Assert.AreEqual(0, associations.First().Stock);
            Assert.IsNull(associations.First().LastTaken);
            Assert.IsTrue(associations.First().Active);
        }

        [TestMethod]
        public async Task ListMissingTest()
        {
            var associations = await _factory.PersonMedications.ListAsync(e => e.MedicationId == _secondMedicationId, 1, int.MaxValue);
            Assert.AreEqual(0, associations.Count);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            await _factory.PersonMedications.UpdateAsync(_associationId, _personId, _secondMedicationId, 2, 10, DateTime.Now, false);
            var associations = await _factory.PersonMedications.ListAsync(a => a.Id == _associationId, 1, int.MaxValue);
            Assert.AreEqual(1, associations.Count);
            Assert.AreEqual(_personId, associations.First().PersonId);
            Assert.AreEqual(_secondMedicationId, associations.First().MedicationId);
            Assert.AreEqual(2, associations.First().DailyDose);
            Assert.AreEqual(10, associations.First().Stock);
            Assert.IsNotNull(associations.First().LastTaken);
            Assert.IsFalse(associations.First().Active);
        }

        [TestMethod]
        public async Task DeactivateTest()
        {
            var association = (await _factory.PersonMedications.ListAsync(x => x.Id == _associationId, 1, int.MaxValue)).First();
            Assert.IsTrue(association.Active);

            await _factory.PersonMedications.DeactivateAsync(_associationId);
            association = (await _factory.PersonMedications.ListAsync(x => x.Id == _associationId, 1, int.MaxValue)).First();
            Assert.IsFalse(association.Active);
        }

        [TestMethod]
        public async Task ActivateTest()
        {
            await _factory.PersonMedications.DeactivateAsync(_associationId);
            var association = (await _factory.PersonMedications.ListAsync(x => x.Id == _associationId, 1, int.MaxValue)).First();
            Assert.IsFalse(association.Active);

            await _factory.PersonMedications.ActivateAsync(_associationId);
            association = (await _factory.PersonMedications.ListAsync(x => x.Id == _associationId, 1, int.MaxValue)).First();
            Assert.IsTrue(association.Active);
        }

        [TestMethod]
        public async Task SetDoseTest()
        {
            await _factory.PersonMedications.SetDoseAsync(_associationId, 3);
            var associations = await _factory.PersonMedications.ListAsync(a => true, 1, int.MaxValue);
            Assert.AreEqual(1, associations.Count);
            Assert.AreEqual(_personId, associations.First().PersonId);
            Assert.AreEqual(_firstMedicationId, associations.First().MedicationId);
            Assert.AreEqual(3, associations.First().DailyDose);
            Assert.AreEqual(0, associations.First().Stock);
            Assert.IsNull(associations.First().LastTaken);
            Assert.IsTrue(associations.First().Active);
        }

        [TestMethod]
        public async Task SetStockTest()
        {
            await _factory.PersonMedications.SetStockAsync(_associationId, 35);
            var associations = await _factory.PersonMedications.ListAsync(a => true, 1, int.MaxValue);
            Assert.AreEqual(1, associations.Count);
            Assert.AreEqual(_personId, associations.First().PersonId);
            Assert.AreEqual(_firstMedicationId, associations.First().MedicationId);
            Assert.AreEqual(1, associations.First().DailyDose);
            Assert.AreEqual(35, associations.First().Stock);
            Assert.IsNull(associations.First().LastTaken);
            Assert.IsTrue(associations.First().Active);
        }

        [TestMethod]
        public async Task SetStockAndDateTakenTest()
        {
            await _factory.PersonMedications.SetStockAsync(_associationId, 35, DateTime.Now);
            var associations = await _factory.PersonMedications.ListAsync(a => true, 1, int.MaxValue);
            Assert.AreEqual(1, associations.Count);
            Assert.AreEqual(_personId, associations.First().PersonId);
            Assert.AreEqual(_firstMedicationId, associations.First().MedicationId);
            Assert.AreEqual(1, associations.First().DailyDose);
            Assert.AreEqual(35, associations.First().Stock);
            Assert.IsNotNull(associations.First().LastTaken);
            Assert.IsTrue(associations.First().Active);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            await _factory.PersonMedications.DeleteAsync(_associationId);
            var associations = await _factory.PersonMedications.ListAsync(a => true, 1, int.MaxValue);
            Assert.AreEqual(0, associations.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(DuplicatePersonMedicationException))]
        public async Task CannotCreateDuplicateAssociationTest()
            => await _factory.PersonMedications.AddAsync(_personId, _firstMedicationId, 1, 0, null);

        [TestMethod]
        [ExpectedException(typeof(DuplicatePersonMedicationException))]
        public async Task CannotUpdateToProduceDuplicateTest()
        {
            var association = await _factory.PersonMedications.AddAsync(_personId, _secondMedicationId, 2, 10, null);
            _ = await _factory.PersonMedications.UpdateAsync(association.Id, _personId, _firstMedicationId, 2, 10, null, true);
        }

        [TestMethod]
        [ExpectedException(typeof(PersonNotFoundException))]
        public async Task CannotCreateAssociationForMissingPersonTest()
            => await _factory.PersonMedications.AddAsync(-1, _secondMedicationId, 1, 0, null);

        [TestMethod]
        [ExpectedException(typeof(MedicationNotFoundException))]
        public async Task CannotCreateAssociationForMissingMedicationTest()
            => await _factory.PersonMedications.AddAsync(_personId, -1, 1, 0, null);

        [TestMethod]
        [ExpectedException(typeof(InvalidDoseException))]
        public async Task CannotCreateAssociationWithInvalidDoseTest()
            => await _factory.PersonMedications.AddAsync(_personId, _secondMedicationId, 0, 0, null);

        [TestMethod]
        [ExpectedException(typeof(InvalidStockLevelException))]
        public async Task CannotCreateAssociationWithInvalidStockLevelTest()
            => await _factory.PersonMedications.AddAsync(_personId, _secondMedicationId, 1, -1, null);

        [TestMethod]
        [ExpectedException(typeof(InvalidDoseDateException))]
        public async Task CannotCreateAssociationWithInvalidDoseDateTest()
            => await _factory.PersonMedications.AddAsync(_personId, _secondMedicationId, 1, 10, DateTime.Now.AddDays(1));

        [TestMethod]
        [ExpectedException(typeof(InvalidDoseException))]
        public async Task CannotSetInvalidDoseTest()
            => await _factory.PersonMedications.SetDoseAsync(_associationId, 0);

        [TestMethod]
        [ExpectedException(typeof(InvalidStockLevelException))]
        public async Task CannotSetInvalidStockTest()
            => await _factory.PersonMedications.SetStockAsync(_associationId, -1);

        [TestMethod]
        [ExpectedException(typeof(InvalidDoseDateException))]
        public async Task CannotSetInvalidDateTakenTest()
            => await _factory.PersonMedications.SetStockAsync(_associationId, 0, DateTime.Now.AddDays(1));
    }
}
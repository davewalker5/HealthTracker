using HealthTracker.Data;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.People
{
    [TestClass]
    public class PersonManagerTest
    {
        private readonly string FirstNames = DataGenerator.RandomFirstNames();
        private readonly string Surname = DataGenerator.RandomSurname();
        private readonly DateTime DateOfBirth = DataGenerator.RandomDateOfBirth(16, 90);
        private readonly decimal Height = DataGenerator.RandomHeight();
        private readonly Gender PersonGender = DataGenerator.RandomGender();

        private readonly string UpdatedFirstNames = DataGenerator.RandomFirstNames();
        private readonly string UpdatedSurname = DataGenerator.RandomSurname();
        private readonly DateTime UpdatedDateOfBirth = DataGenerator.RandomDateOfBirth(16, 90);
        private readonly decimal UpdatedHeight = DataGenerator.RandomHeight();
        private readonly Gender UpdatedPersonGender = DataGenerator.RandomGender();

        private IHealthTrackerFactory _factory;
        private int _personId;

        [TestInitialize]
        public void TestInitialize()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            _personId = Task.Run(() => _factory.People.AddAsync(FirstNames, Surname, DateOfBirth, Height, PersonGender)).Result.Id;
        }

        [TestMethod]
        public async Task AddAndGetTest()
        {
            var person = await _factory.People.GetAsync(a => a.Id == _personId);
            Assert.IsNotNull(person);
            Assert.AreEqual(_personId, person.Id);
            Assert.AreEqual(FirstNames, person.FirstNames);
            Assert.AreEqual(Surname, person.Surname);
            Assert.AreEqual(DateOfBirth, person.DateOfBirth);
            Assert.AreEqual(Height, person.Height);
            Assert.AreEqual(PersonGender, person.Gender);
        }
        
        [TestMethod]
        public async Task GetMissingTest()
        {
            var person = await _factory.People.GetAsync(a => a.Id == (10 * _personId));
            Assert.IsNull(person);
        }

        [TestMethod]
        public async Task ListAllTest()
        {
            var people = await _factory.People.ListAsync(x => true);
            Assert.AreEqual(1, people.Count);
            Assert.AreEqual(FirstNames, people.First().FirstNames);
            Assert.AreEqual(Surname, people.First().Surname);
            Assert.AreEqual(DateOfBirth, people.First().DateOfBirth);
            Assert.AreEqual(Height, people.First().Height);
            Assert.AreEqual(PersonGender, people.First().Gender);
        }

        [TestMethod]
        public async Task ListMissingTest()
        {
            var people = await _factory.People.ListAsync(e => e.Surname == "Missing");
            Assert.AreEqual(0, people.Count);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            await _factory.People.UpdateAsync(_personId, UpdatedFirstNames, UpdatedSurname, UpdatedDateOfBirth, UpdatedHeight, UpdatedPersonGender);
            var person = await _factory.People.GetAsync(a => a.Id == _personId);
            Assert.IsNotNull(person);
            Assert.AreEqual(_personId, person.Id);
            Assert.AreEqual(UpdatedFirstNames, person.FirstNames);
            Assert.AreEqual(UpdatedSurname, person.Surname);
            Assert.AreEqual(UpdatedDateOfBirth, person.DateOfBirth);
            Assert.AreEqual(UpdatedHeight, person.Height);
            Assert.AreEqual(UpdatedPersonGender, person.Gender);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            await _factory.People.DeleteAsync(_personId);
            var people = await _factory.People.ListAsync(a => true);
            Assert.AreEqual(0, people.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(PersonInUseException))]
        public async Task CannotDeleteWithWeightMeasurementTest()
        {
            var context = _factory.Context as HealthTrackerDbContext;
            await context.WeightMeasurements.AddAsync(new WeightMeasurement{ PersonId = _personId, Date = DateTime.Now});
            await context.SaveChangesAsync();
            await _factory.People.DeleteAsync(_personId);
        }

        [TestMethod]
        [ExpectedException(typeof(PersonInUseException))]
        public async Task CannotDeleteWithBloodPressureMeasurementTest()
        {
            var context = _factory.Context as HealthTrackerDbContext;
            await context.BloodPressureMeasurements.AddAsync(new BloodPressureMeasurement{ PersonId = _personId, Date = DateTime.Now});
            await context.SaveChangesAsync();
            await _factory.People.DeleteAsync(_personId);
        }

        [TestMethod]
        [ExpectedException(typeof(PersonInUseException))]
        public async Task CannotDeleteWithExerciseMeasurementTest()
        {
            var context = _factory.Context as HealthTrackerDbContext;
            await context.ExerciseMeasurements.AddAsync(new ExerciseMeasurement{ PersonId = _personId, Date = DateTime.Now, Distance = 0});
            await context.SaveChangesAsync();
            await _factory.People.DeleteAsync(_personId);
        }
    }
}
using HealthTracker.Data;
using HealthTracker.Enumerations.Enumerations;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.BloodOxygenSaturation
{
    [TestClass]
    public class BloodOxygenSaturationAssessorTest
    {
        private readonly string FirstNames = DataGenerator.RandomFirstNames();
        private readonly string Surname = DataGenerator.RandomSurname();

        private readonly List<BloodOxygenSaturationBand> _bands = [
            new() { Name = "Normal", MinimumSPO2 = 93, MaximumSPO2 = decimal.MaxValue, MinimumAge = 0, MaximumAge = 12 },
            new() { Name = "Low", MinimumSPO2 = 0, MaximumSPO2 = 92.99M, MinimumAge = 0, MaximumAge = 12 },
            new() { Name = "Normal", MinimumSPO2 = 96, MaximumSPO2 = decimal.MaxValue, MinimumAge = 13, MaximumAge = 69 },
            new() { Name = "Low", MinimumSPO2 = 0, MaximumSPO2 = 95.99M, MinimumAge = 13, MaximumAge = 69 },
            new() { Name = "Normal", MinimumSPO2 = 94, MaximumSPO2 = decimal.MaxValue, MinimumAge = 70, MaximumAge = int.MaxValue },
            new() { Name = "Low", MinimumSPO2 = 0, MaximumSPO2 = 99.33M, MinimumAge = 70, MaximumAge = int.MaxValue },
        ];

        private IHealthTrackerFactory _factory;

        [TestInitialize]
        public async Task Initialise()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            foreach (var band in _bands)
            {
                await context.BloodOxygenSaturationBands.AddAsync(band);
            }
            await context.SaveChangesAsync();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
        }

        [TestMethod]
        public async Task ChildNormalTest()
        {
            var dob = DataGenerator.RandomDateOfBirth(1, 12);
            var person = await _factory.People.AddAsync(FirstNames, Surname, dob, 0, Gender.Unspecified);
            Assert.IsTrue(person.AgeInYears() <= 12);

            var measurement = new BloodOxygenSaturationMeasurement { PersonId = person.Id, Date = DateTime.Now, Percentage = 93M };
            await _factory.BloodOxygenSaturationAssessor.Assess([measurement]);
            Assert.AreEqual("Normal", measurement.Assessment);
        }

        [TestMethod]
        public async Task ChildLowTest()
        {
            var dob = DataGenerator.RandomDateOfBirth(1, 12);
            var person = await _factory.People.AddAsync(FirstNames, Surname, dob, 0, Gender.Unspecified);
            Assert.IsTrue(person.AgeInYears() <= 12);

            var measurement = new BloodOxygenSaturationMeasurement { PersonId = person.Id, Date = DateTime.Now, Percentage = 92M };
            await _factory.BloodOxygenSaturationAssessor.Assess([measurement]);
            Assert.AreEqual("Low", measurement.Assessment);
        }

        [TestMethod]
        public async Task AdultNormalTest()
        {
            var dob = DataGenerator.RandomDateOfBirth(35, 69);
            var person = await _factory.People.AddAsync(FirstNames, Surname, dob, 0, Gender.Unspecified);
            Assert.IsTrue(person.AgeInYears() >= 35 && person.AgeInYears() <= 69);

            var measurement = new BloodOxygenSaturationMeasurement { PersonId = person.Id, Date = DateTime.Now, Percentage = 96M };
            await _factory.BloodOxygenSaturationAssessor.Assess([measurement]);
            Assert.AreEqual("Normal", measurement.Assessment);
        }

        [TestMethod]
        public async Task AdultLowTest()
        {
            var dob = DataGenerator.RandomDateOfBirth(35, 69);
            var person = await _factory.People.AddAsync(FirstNames, Surname, dob, 0, Gender.Unspecified);
            Assert.IsTrue(person.AgeInYears() >= 35 && person.AgeInYears() <= 69);

            var measurement = new BloodOxygenSaturationMeasurement { PersonId = person.Id, Date = DateTime.Now, Percentage = 94M };
            await _factory.BloodOxygenSaturationAssessor.Assess([measurement]);
            Assert.AreEqual("Low", measurement.Assessment);
        }

        [TestMethod]
        public async Task OlderAdultNormalTest()
        {
            var dob = DataGenerator.RandomDateOfBirth(70, 100);
            var person = await _factory.People.AddAsync(FirstNames, Surname, dob, 0, Gender.Unspecified);
            Assert.IsTrue(person.AgeInYears() >= 70 && person.AgeInYears() <= 100);

            var measurement = new BloodOxygenSaturationMeasurement { PersonId = person.Id, Date = DateTime.Now, Percentage = 94M };
            await _factory.BloodOxygenSaturationAssessor.Assess([measurement]);
            Assert.AreEqual("Normal", measurement.Assessment);
        }

        [TestMethod]
        public async Task OlderAdultLowTest()
        {
            var dob = DataGenerator.RandomDateOfBirth(70, 100);
            var person = await _factory.People.AddAsync(FirstNames, Surname, dob, 0, Gender.Unspecified);
            Assert.IsTrue(person.AgeInYears() >= 70 && person.AgeInYears() <= 100);

            var measurement = new BloodOxygenSaturationMeasurement { PersonId = person.Id, Date = DateTime.Now, Percentage = 93M };
            await _factory.BloodOxygenSaturationAssessor.Assess([measurement]);
            Assert.AreEqual("Low", measurement.Assessment);
        }
    }
}
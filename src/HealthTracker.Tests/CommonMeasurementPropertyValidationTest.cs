using HealthTracker.Data;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Exceptions;
using HealthTracker.DataExchange.Import;
using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests
{
    [TestClass]
    public class CommonMeasurementPropertyValidationTest
    {
        private Person _person;
        private BloodPressureMeasurement _measurement;
        private const string Assessment = "Optimal";

        private BloodPressureMeasurementImporter _importer;

        [TestInitialize]
        public async Task Initialise()
        {
            _person = DataGenerator.RandomPerson(16, 90);
            _measurement = DataGenerator.RandomBloodPressureMeasurement(_person.Id, 2024, 0, 119, 0, 79);

            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            await context.People.AddAsync(_person);
            await context.SaveChangesAsync();

            var logger = new Mock<IHealthTrackerLogger>();
            var factory = new HealthTrackerFactory(context, null, logger.Object);
            _importer = new BloodPressureMeasurementImporter(factory, ExportableBloodPressureMeasurement.CsvRecordPattern);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidPersonIdTest()
        {
            var record = $@"""{0}"",""{_person.Name()}"",""{_measurement.Date:dd/MM/yyyy}"",""{_measurement.Systolic}"",""{_measurement.Diastolic}"",""{Assessment}""";
            var filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(filePath, ["", record]);

            await _importer.ImportAsync(filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidNameTest()
        {
            var record = $@"""{_person.Id}"","""",""{_measurement.Date:dd/MM/yyyy}"",""{_measurement.Systolic}"",""{_measurement.Diastolic}"",""{Assessment}""";
            var filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(filePath, ["", record]);

            await _importer.ImportAsync(filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidDateTest()
        {
            var date = DateTime.Now.AddDays(1);
            var record = $@"""{_person.Id}"",""{_person.Name()}"",""{date:dd/MM/yyyy}"",""{_measurement.Systolic}"",""{_measurement.Diastolic}"",""{Assessment}""";
            var filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(filePath, ["", record]);

            await _importer.ImportAsync(filePath);
        }
    }
}
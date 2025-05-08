using HealthTracker.Data;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Exceptions;
using HealthTracker.DataExchange.Extensions;
using HealthTracker.DataExchange.Import;
using HealthTracker.DataExchange.Interfaces;
using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.BloodPressure
{
    [TestClass]
    public class BloodPressureMeasurementImportTest
    {
        private Person _person;
        private ExportableBloodPressureMeasurement _measurement;
        private string _filePath;

        private IHealthTrackerFactory _factory;
        private IBloodPressureMeasurementImporter _importer;

        [TestInitialize]
        public async Task Initialise()
        {
            _person = DataGenerator.RandomPerson(16, 90);
            _measurement = DataGenerator.RandomBloodPressureMeasurement(_person.Id, 2024, 1, 119, 1, 79).ToExportable([_person]);

            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            await context.People.AddAsync(_person);
            await context.SaveChangesAsync();

            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            _importer = new BloodPressureMeasurementImporter(_factory, ExportableBloodPressureMeasurement.CsvRecordPattern);
        }

        [TestCleanup]
        public void CleanUp()
        {
            if (!string.IsNullOrEmpty(_filePath) && File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }

        [TestMethod]
        public void ConvertSingleObjectFromExportable()
        {
            var measurement = _measurement.FromExportable();
            Assert.AreEqual(_measurement.PersonId, measurement.PersonId);
            Assert.AreEqual(_measurement.Date, measurement.Date);
            Assert.AreEqual(_measurement.Systolic, measurement.Systolic);
            Assert.AreEqual(_measurement.Diastolic, measurement.Diastolic);
        }

        [TestMethod]
        public void ConvertCollectionFromExportable()
        {
            List<ExportableBloodPressureMeasurement> exportable = [_measurement];
            var measurements = exportable.FromExportable();
            Assert.AreEqual(_measurement.PersonId, measurements.First().PersonId);
            Assert.AreEqual(_measurement.Date, measurements.First().Date);
            Assert.AreEqual(_measurement.Systolic, measurements.First().Systolic);
            Assert.AreEqual(_measurement.Diastolic, measurements.First().Diastolic);
        }

        [TestMethod]
        public async Task ImportMeasurementsTest()
        {
            var record = $@"""{_person.Id}"",""{_person.Name}"",""{_measurement.Date:dd/MM/yyyy}"",""{_measurement.Systolic}"",""{_measurement.Diastolic}"",""Optimal""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var measurements = await _factory.BloodPressureMeasurements.ListAsync(x => true);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(_measurement.PersonId, measurements[0].PersonId);
            Assert.AreEqual(_measurement.Systolic, measurements[0].Systolic);
            Assert.AreEqual(_measurement.Diastolic, measurements[0].Diastolic);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRecordFormatException))]
        public async Task InvalidRecordFormatTest()
        {
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", "Invalid record format"]);
            await _importer.ImportAsync(_filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidSystolicTest()
        {
            var record = $@"""{_person.Id}"",""{_person.Name}"",""{_measurement.Date:dd/MM/yyyy}"",""{0}"",""{_measurement.Diastolic}"",""Optimal""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);
            await _importer.ImportAsync(_filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidDiastolicTest()
        {
            var record = $@"""{_person.Id}"",""{_person.Name}"",""{_measurement.Date:dd/MM/yyyy}"",""{_measurement.Systolic}"",""{0}"",""Optimal""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);
            await _importer.ImportAsync(_filePath);
        }
    }
}
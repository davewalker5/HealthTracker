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

namespace HealthTracker.Tests.Cholesterol
{
    [TestClass]
    public class CholesterolMeasurementImportTest
    {
        private Person _person;
        private ExportableCholesterolMeasurement _measurement;
        private string _filePath;

        private IHealthTrackerFactory _factory;
        private ICholesterolMeasurementImporter _importer;

        [TestInitialize]
        public async Task Initialise()
        {
            _person = DataGenerator.RandomPerson(16, 90);
            _measurement = DataGenerator.RandomCholesterolMeasurement(_person.Id, 2024).ToExportable([_person]);

            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            await context.People.AddAsync(_person);
            await context.SaveChangesAsync();

            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            _importer = new CholesterolMeasurementImporter(_factory, ExportableCholesterolMeasurement.CsvRecordPattern);
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
            Assert.AreEqual(_person.Id, measurement.PersonId);
            Assert.AreEqual(_measurement.Date, measurement.Date);
            Assert.AreEqual(_measurement.Total, measurement.Total);
            Assert.AreEqual(_measurement.HDL, measurement.HDL);
            Assert.AreEqual(_measurement.LDL, measurement.LDL);
            Assert.AreEqual(_measurement.Triglycerides, measurement.Triglycerides);
        }

        [TestMethod]
        public void ConvertCollectionFromExportable()
        {
            List<ExportableCholesterolMeasurement> exportable = [_measurement];
            var measurements = exportable.FromExportable();
            Assert.AreEqual(_person.Id, measurements.First().PersonId);
            Assert.AreEqual(_measurement.Date, measurements.First().Date);
            Assert.AreEqual(_measurement.Total, measurements.First().Total);
            Assert.AreEqual(_measurement.HDL, measurements.First().HDL);
            Assert.AreEqual(_measurement.LDL, measurements.First().LDL);
            Assert.AreEqual(_measurement.Triglycerides, measurements.First().Triglycerides);
        }

        [TestMethod]
        public async Task ImportMeasurementsTest()
        {
            var record = $@"""{_person.Id}"",""{_person.Name}"",""{_measurement.Date:dd/MM/yyyy}"",""{_measurement.Total}"",""{_measurement.HDL}"",""{_measurement.LDL}"",""{_measurement.Triglycerides}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var measurements = await _factory.CholesterolMeasurements.ListAsync(x => true);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(_person.Id, measurements.First().PersonId);
            Assert.AreEqual(_measurement.Date, measurements.First().Date);
            Assert.AreEqual(_measurement.Total, measurements.First().Total);
            Assert.AreEqual(_measurement.HDL, measurements.First().HDL);
            Assert.AreEqual(_measurement.LDL, measurements.First().LDL);
            Assert.AreEqual(_measurement.Triglycerides, measurements.First().Triglycerides);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRecordFormatException))]
        public async Task InvalidRecordFormatTest()
        {
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", "Invalid record format"]);

            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var factory = new HealthTrackerFactory(context, null, null);
            var importer = new CholesterolMeasurementImporter(factory, ExportableCholesterolMeasurement.CsvRecordPattern);
            await importer.ImportAsync(_filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidTotalTest()
        {
            var record = $@"""{_person.Id}"",""{_person.Name}"",""{_measurement.Date:dd/MM/yyyy}"",""{0}"",""{_measurement.HDL}"",""{_measurement.LDL}"",""{_measurement.Triglycerides}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidHDLTest()
        {
            var record = $@"""{_person.Id}"",""{_person.Name}"",""{_measurement.Date:dd/MM/yyyy}"",""{_measurement.Total}"",""{0}"",""{_measurement.LDL}"",""{_measurement.Triglycerides}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidLDLTest()
        {
            var record = $@"""{_person.Id}"",""{_person.Name}"",""{_measurement.Date:dd/MM/yyyy}"",""{_measurement.Total}"",""{_measurement.HDL}"",""{0}"",""{_measurement.Triglycerides}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidTriglyceridesTest()
        {
            var record = $@"""{_person.Id}"",""{_person.Name}"",""{_measurement.Date:dd/MM/yyyy}"",""{_measurement.Total}"",""{_measurement.HDL}"",""{_measurement.LDL}"",""{0}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);
        }
    }
}
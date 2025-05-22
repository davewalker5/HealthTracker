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

namespace HealthTracker.Tests.BloodGlucose
{
    [TestClass]
    public class BloodGlucoseMeasurementImportTest
    {
        private IHealthTrackerFactory _factory;
        private IBloodGlucoseMeasurementImporter _importer;
        private Person _person;
        private ExportableBloodGlucoseMeasurement _measurement;
        private string _filePath;

        [TestInitialize]
        public async Task Initialise()
        {
            _person = DataGenerator.RandomPerson(16, 90);
            _measurement = DataGenerator.RandomBloodGlucoseMeasurement(_person.Id, 2024).ToExportable([_person]);

            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            await context.People.AddAsync(_person);
            await context.SaveChangesAsync();

            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            _importer = new BloodGlucoseMeasurementImporter(_factory, ExportableBloodGlucoseMeasurement.CsvRecordPattern);
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
            Assert.AreEqual(_measurement.Level, measurement.Level);
        }

        [TestMethod]
        public void ConvertCollectionFromExportable()
        {
            List<ExportableBloodGlucoseMeasurement> exportable = [_measurement];
            var measurements = exportable.FromExportable();
            Assert.AreEqual(_measurement.PersonId, measurements.First().PersonId);
            Assert.AreEqual(_measurement.Date, measurements.First().Date);
            Assert.AreEqual(_measurement.Level, measurements.First().Level);
        }

        [TestMethod]
        public async Task ImportMeasurementsTest()
        {
            var record = $@"""{_person.Id}"",""{_person.FirstNames} {_person.Surname}"",""{_measurement.Date:yyyy-MM-dd HH:mm:ss}"",""{_measurement.Level}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var measurements = await _factory.BloodGlucoseMeasurements.ListAsync(x => true, 1, int.MaxValue);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(_measurement.PersonId, measurements.First().PersonId);
            Assert.AreEqual(_measurement.Date, measurements.First().Date);
            Assert.AreEqual(_measurement.Level, measurements.First().Level);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRecordFormatException))]
        public async Task InvalidRecordFormatTest()
        {
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", "Invalid record format"]);

            await _importer.ImportAsync(_filePath);
        }
    }
}
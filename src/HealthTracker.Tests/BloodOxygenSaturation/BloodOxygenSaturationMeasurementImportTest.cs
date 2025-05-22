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

namespace HealthTracker.Tests.BloodOxygenSaturation
{
    [TestClass]
    public class BloodOxygenSaturationMeasurementImportTest
    {
        private IHealthTrackerFactory _factory;
        private IBloodOxygenSaturationMeasurementImporter _importer;
        private Person _person;
        private ExportableBloodOxygenSaturationMeasurement _measurement;
        private string _filePath;

        [TestInitialize]
        public async Task Initialise()
        {
            _person = DataGenerator.RandomPerson(16, 90);
            _measurement = DataGenerator.RandomSPO2Measurement(_person.Id, 2024).ToExportable([_person]);

            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            await context.People.AddAsync(_person);
            await context.SaveChangesAsync();

            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            _importer = new BloodOxygenSaturationMeasurementImporter(_factory, ExportableBloodOxygenSaturationMeasurement.CsvRecordPattern);
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
            Assert.AreEqual(_measurement.Percentage, measurement.Percentage);
            Assert.AreEqual(_measurement.Assessment, measurement.Assessment);
        }

        [TestMethod]
        public void ConvertCollectionFromExportable()
        {
            List<ExportableBloodOxygenSaturationMeasurement> exportable = [_measurement];
            var measurements = exportable.FromExportable();
            Assert.AreEqual(_measurement.PersonId, measurements.First().PersonId);
            Assert.AreEqual(_measurement.Date, measurements.First().Date);
            Assert.AreEqual(_measurement.Percentage, measurements.First().Percentage);
            Assert.AreEqual(_measurement.Assessment, measurements.First().Assessment);
        }

        [TestMethod]
        public async Task ImportMeasurementsTest()
        {
            var record = $@"""{_person.Id}"",""{_person.FirstNames} {_person.Surname}"",""{_measurement.Date:dd/MM/yyyy}"",""{_measurement.Percentage}"",""{_measurement.Assessment}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var measurements = await _factory.BloodOxygenSaturationMeasurements.ListAsync(x => true, 1, int.MaxValue);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(_measurement.PersonId, measurements.First().PersonId);
            Assert.AreEqual(_measurement.Date, measurements.First().Date);
            Assert.AreEqual(_measurement.Percentage, measurements.First().Percentage);
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
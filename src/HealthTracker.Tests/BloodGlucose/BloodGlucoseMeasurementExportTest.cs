using HealthTracker.Data;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Export;
using HealthTracker.DataExchange.Extensions;
using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.BloodGlucose
{
    [TestClass]
    public class BloodGlucoseMeasurementExportTest
    {
        private Person _person;
        private BloodGlucoseMeasurement _measurement;
        private string _filePath;

        [TestInitialize]
        public void Initialise()
        {
            _person = DataGenerator.RandomPerson(16, 90);
            _measurement = DataGenerator.RandomBloodGlucoseMeasurement(_person.Id, 2024);
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
        public void ConvertSingleObjectToExportable()
        {
            var exportable = _measurement.ToExportable([_person]);
            Assert.AreEqual(_measurement.PersonId, exportable.PersonId);
            Assert.AreEqual($"{_person.FirstNames} {_person.Surname}", exportable.Name);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_measurement.Level, exportable.Level);
        }

        [TestMethod]
        public void ConvertCollectionToExportable()
        {
            List<BloodGlucoseMeasurement> measurements = [_measurement];
            var exportable = measurements.ToExportable([_person]);
            Assert.AreEqual(_measurement.PersonId, exportable.First().PersonId);
            Assert.AreEqual($"{_person.FirstNames} {_person.Surname}", exportable.First().Name);
            Assert.AreEqual(_measurement.Date, exportable.First().Date);
            Assert.AreEqual(_measurement.Level, exportable.First().Level);
        }

        [TestMethod]
        public void FromCsvRecordTest()
        {
            var record = $@"""{_person.Id}"",""{_person.FirstNames} {_person.Surname}"",""{_measurement.Date:dd-MMM-yyyy HH:mm:ss}"",""{_measurement.Level}""";
            var exportable = ExportableBloodGlucoseMeasurement.FromCsv(record);
            Assert.AreEqual(_measurement.PersonId, exportable.PersonId);
            Assert.AreEqual($"{_person.FirstNames} {_person.Surname}", exportable.Name);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_measurement.Level, exportable.Level);
        }

        [TestMethod]
        public async Task ExportAllMeasurementsTest()
        {
            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            await context.People.AddAsync(_person);
            await context.BloodGlucoseMeasurements.AddAsync(_measurement);
            await context.SaveChangesAsync();

            var logger = new Mock<IHealthTrackerLogger>();
            var factory = new HealthTrackerFactory(context, null, logger.Object);
            var exporter = new BloodGlucoseMeasurementExporter(factory);

            _filePath = DataGenerator.TemporaryCsvFilePath();
            List<BloodGlucoseMeasurement> measurements = [_measurement];
            await exporter.ExportAsync(measurements, _filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var records = File.ReadAllLines(_filePath);
            Assert.AreEqual(2, records.Length);

            var exportable = ExportableBloodGlucoseMeasurement.FromCsv(records[1]);
            Assert.AreEqual(_measurement.PersonId, exportable.PersonId);
            Assert.AreEqual($"{_person.FirstNames} {_person.Surname}", exportable.Name);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_measurement.Level, exportable.Level);
        }

        [TestMethod]
        public async Task ExportMeasurementsForPersonTest()
        {
            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            await context.People.AddAsync(_person);
            await context.BloodGlucoseMeasurements.AddAsync(_measurement);
            await context.SaveChangesAsync();

            var logger = new Mock<IHealthTrackerLogger>();
            var factory = new HealthTrackerFactory(context, null, logger.Object);
            var exporter = new BloodGlucoseMeasurementExporter(factory);

            _filePath = DataGenerator.TemporaryCsvFilePath();
            await exporter.ExportAsync(_person.Id, null, null, _filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var records = File.ReadAllLines(_filePath);
            Assert.AreEqual(2, records.Length);

            var exportable = ExportableBloodGlucoseMeasurement.FromCsv(records[1]);
            Assert.AreEqual(_measurement.PersonId, exportable.PersonId);
            Assert.AreEqual($"{_person.FirstNames} {_person.Surname}", exportable.Name);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_measurement.Level, exportable.Level);
        }
    }
}
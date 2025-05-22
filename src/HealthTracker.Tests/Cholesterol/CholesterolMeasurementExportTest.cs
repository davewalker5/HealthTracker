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

namespace HealthTracker.Tests.Cholesterol
{
    [TestClass]
    public class CholesterolMeasurementExportTest
    {
        private Person _person;
        private CholesterolMeasurement _measurement;
        private string _filePath;

        [TestInitialize]
        public void Initialise()
        {
            _person = DataGenerator.RandomPerson(16, 90);
            _measurement = DataGenerator.RandomCholesterolMeasurement(_person.Id, 2024);
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
            Assert.AreEqual(_person.Id, exportable.PersonId);
            Assert.AreEqual(_person.Name, exportable.Name);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_measurement.Total, exportable.Total);
            Assert.AreEqual(_measurement.HDL, exportable.HDL);
            Assert.AreEqual(_measurement.LDL, exportable.LDL);
            Assert.AreEqual(_measurement.Triglycerides, exportable.Triglycerides);
        }

        [TestMethod]
        public void ConvertCollectionToExportable()
        {
            List<CholesterolMeasurement> measurements = [_measurement];
            var exportable = measurements.ToExportable([_person]);
            Assert.AreEqual(_person.Id, exportable.First().PersonId);
            Assert.AreEqual(_person.Name, exportable.First().Name);
            Assert.AreEqual(_measurement.Date, exportable.First().Date);
            Assert.AreEqual(_measurement.Total, exportable.First().Total);
            Assert.AreEqual(_measurement.HDL, exportable.First().HDL);
            Assert.AreEqual(_measurement.LDL, exportable.First().LDL);
            Assert.AreEqual(_measurement.Triglycerides, exportable.First().Triglycerides);
        }

        [TestMethod]
        public void FromCsvRecordTest()
        {
            var record = $@"""{_person.Id}"",""{_person.Name}"",""{_measurement.Date:dd/MM/yyyy}"",""{_measurement.Total}"",""{_measurement.HDL}"",""{_measurement.LDL}"",""{_measurement.Triglycerides}""";
            var exportable = ExportableCholesterolMeasurement.FromCsv(record);
            Assert.AreEqual(_person.Id, exportable.PersonId);
            Assert.AreEqual(_person.Name, exportable.Name);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_measurement.Total, exportable.Total);
            Assert.AreEqual(_measurement.HDL, exportable.HDL);
            Assert.AreEqual(_measurement.LDL, exportable.LDL);
            Assert.AreEqual(_measurement.Triglycerides, exportable.Triglycerides);
        }

        [TestMethod]
        public async Task ExportAllMeasurementsTest()
        {
            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            await context.People.AddAsync(_person);
            await context.CholesterolMeasurements.AddAsync(_measurement);
            await context.SaveChangesAsync();

            var logger = new Mock<IHealthTrackerLogger>();
            var factory = new HealthTrackerFactory(context, null, logger.Object);
            var exporter = new CholesterolMeasurementExporter(factory);

            _filePath = DataGenerator.TemporaryCsvFilePath();
            List<CholesterolMeasurement> measurements = [_measurement];
            await exporter.ExportAsync(measurements, _filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var records = File.ReadAllLines(_filePath);
            Assert.AreEqual(2, records.Length);

            var exportable = ExportableCholesterolMeasurement.FromCsv(records[1]);
            Assert.AreEqual(_person.Id, exportable.PersonId);
            Assert.AreEqual(_person.Name, exportable.Name);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_measurement.Total, exportable.Total);
            Assert.AreEqual(_measurement.HDL, exportable.HDL);
            Assert.AreEqual(_measurement.LDL, exportable.LDL);
            Assert.AreEqual(_measurement.Triglycerides, exportable.Triglycerides);
        }

        [TestMethod]
        public async Task ExportMeasurementsForPersonTest()
        {
            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            await context.People.AddAsync(_person);
            await context.CholesterolMeasurements.AddAsync(_measurement);
            await context.SaveChangesAsync();

            var logger = new Mock<IHealthTrackerLogger>();
            var factory = new HealthTrackerFactory(context, null, logger.Object);
            var exporter = new CholesterolMeasurementExporter(factory);

            _filePath = DataGenerator.TemporaryCsvFilePath();
            await exporter.ExportAsync(_person.Id, null, null, _filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var records = File.ReadAllLines(_filePath);
            Assert.AreEqual(2, records.Length);

            var exportable = ExportableCholesterolMeasurement.FromCsv(records[1]);
            Assert.AreEqual(_person.Id, exportable.PersonId);
            Assert.AreEqual(_person.Name, exportable.Name);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_measurement.Total, exportable.Total);
            Assert.AreEqual(_measurement.HDL, exportable.HDL);
            Assert.AreEqual(_measurement.LDL, exportable.LDL);
            Assert.AreEqual(_measurement.Triglycerides, exportable.Triglycerides);
        }
    }
}
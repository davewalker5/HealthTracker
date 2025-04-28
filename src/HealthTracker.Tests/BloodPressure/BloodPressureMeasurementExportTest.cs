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

namespace HealthTracker.Tests.BloodPressure
{
    [TestClass]
    public class BloodPressureMeasurementExportTest
    {
        private Person _person;
        private BloodPressureMeasurement _measurement;
        private string _filePath;

        [TestInitialize]
        public void Initialise()
        {
            _person = DataGenerator.RandomPerson(16, 90);
            _measurement = DataGenerator.RandomBloodPressureMeasurement(_person.Id, 2024, 0, 119, 0, 79);
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
            Assert.AreEqual(_person.Name(), exportable.Name);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_measurement.Systolic, exportable.Systolic);
            Assert.AreEqual(_measurement.Diastolic, exportable.Diastolic);
        }

        [TestMethod]
        public void ConvertCollectionToExportable()
        {
            List<BloodPressureMeasurement> measurements = [_measurement];
            var exportable = measurements.ToExportable([_person]);
            Assert.AreEqual(_person.Id, exportable.First().PersonId);
            Assert.AreEqual(_person.Name(), exportable.First().Name);
            Assert.AreEqual(_measurement.Date, exportable.First().Date);
            Assert.AreEqual(_measurement.Systolic, exportable.First().Systolic);
            Assert.AreEqual(_measurement.Diastolic, exportable.First().Diastolic);
        }

        [TestMethod]
        public void FromCsvRecordTest()
        {
            var record = $@"""{_person.Id}"",""{_person.Name()}"",""{_measurement.Date:dd/MM/yyyy}"",""{_measurement.Systolic}"",""{_measurement.Diastolic}"",""Optimal""";
            var exportable = ExportableBloodPressureMeasurement.FromCsv(record);
            Assert.AreEqual(_person.Id, exportable.PersonId);
            Assert.AreEqual(_person.Name(), exportable.Name);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_measurement.Systolic, exportable.Systolic);
            Assert.AreEqual(_measurement.Diastolic, exportable.Diastolic);
        }

        [TestMethod]
        public async Task ExportAllMeasurementsTest()
        {
            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            await context.People.AddAsync(_person);
            await context.BloodPressureMeasurements.AddAsync(_measurement);
            await context.SaveChangesAsync();

            var logger = new Mock<IHealthTrackerLogger>();
            var factory = new HealthTrackerFactory(context, null, logger.Object);
            var exporter = new BloodPressureMeasurementExporter(factory);

            _filePath = DataGenerator.TemporaryCsvFilePath();
            List<BloodPressureMeasurement> measurements = [_measurement];
            await exporter.ExportAsync(measurements, _filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var records = File.ReadAllLines(_filePath);
            Assert.AreEqual(2, records.Length);

            var exportable = ExportableBloodPressureMeasurement.FromCsv(records[1]);
            Assert.AreEqual(_person.Id, exportable.PersonId);
            Assert.AreEqual(_person.Name(), exportable.Name);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_measurement.Systolic, exportable.Systolic);
            Assert.AreEqual(_measurement.Diastolic, exportable.Diastolic);
        }

        [TestMethod]
        public async Task ExportMeasurementsForPersonTest()
        {
            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            await context.People.AddAsync(_person);
            await context.BloodPressureMeasurements.AddAsync(_measurement);
            await context.SaveChangesAsync();

            var logger = new Mock<IHealthTrackerLogger>();
            var factory = new HealthTrackerFactory(context, null, logger.Object);
            var exporter = new BloodPressureMeasurementExporter(factory);

            _filePath = DataGenerator.TemporaryCsvFilePath();
            await exporter.ExportAsync(_person.Id, null, null, _filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var records = File.ReadAllLines(_filePath);
            Assert.AreEqual(2, records.Length);

            var exportable = ExportableBloodPressureMeasurement.FromCsv(records[1]);
            Assert.AreEqual(_person.Id, exportable.PersonId);
            Assert.AreEqual(_person.Name(), exportable.Name);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_measurement.Systolic, exportable.Systolic);
            Assert.AreEqual(_measurement.Diastolic, exportable.Diastolic);
        }
    }
}
using HealthTracker.Data;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Exceptions;
using HealthTracker.DataExchange.Extensions;
using HealthTracker.DataExchange.Import;
using HealthTracker.DataExchange.Interfaces;
using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.Logic.Extensions;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.BeverageConsumption
{
    [TestClass]
    public class BeverageConsumptionMeasurementImportTest
    {
        private readonly Person _person = DataGenerator.RandomPerson(16, 90);
        private readonly Beverage _beverage = DataGenerator.RandomBeverage();
        private ExportableBeverageConsumptionMeasurement _measurement;
        private string _filePath;

        private IHealthTrackerFactory _factory;
        private IBeverageConsumptionMeasurementImporter _importer;

        [TestInitialize]
        public async Task Initialise()
        {
            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            await context.People.AddAsync(_person);
            await context.Beverages.AddAsync(_beverage);
            await context.SaveChangesAsync();

            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            _importer = new BeverageConsumptionMeasurementImporter(_factory, ExportableBeverageConsumptionMeasurement.CsvRecordPattern);

            _measurement = DataGenerator.RandomBeverageConsumptionMeasurement(_person.Id, _beverage.Id, 2024).ToExportable([_person], [_beverage]);
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
            var measurement = _measurement.FromExportable([_beverage]);
            Assert.AreEqual(_person.Id, measurement.PersonId);
            Assert.AreEqual(_beverage.Id, measurement.BeverageId);
            Assert.AreEqual(_measurement.Date, measurement.Date);
            Assert.AreEqual(_measurement.Measure, (int)measurement.Measure);
            Assert.AreEqual(_measurement.Quantity, measurement.Quantity);
            Assert.AreEqual(_measurement.ABV, measurement.ABV);
        }

        [TestMethod]
        public void ConvertCollectionFromExportable()
        {
            List<ExportableBeverageConsumptionMeasurement> exportable = [_measurement];
            var measurements = exportable.FromExportable([_beverage]);
            Assert.AreEqual(_person.Id, measurements.First().PersonId);
            Assert.AreEqual(_beverage.Id, measurements.First().BeverageId);
            Assert.AreEqual(_measurement.Date, measurements.First().Date);
            Assert.AreEqual(_measurement.Measure, (int)measurements.First().Measure);
            Assert.AreEqual(_measurement.Quantity, measurements.First().Quantity);
            Assert.AreEqual(_measurement.ABV, measurements.First().ABV);
        }

        [TestMethod]
        public async Task ImportMeasurementsTest()
        {
            var record = $@"""{_person.Id}"",""{_person.Name}"",""{_measurement.Date:dd-MMM-yyyy HH:mm:ss}"",""{_beverage.Id}"",""{_beverage.Name}"",""{(int)_measurement.Measure}"",""{_measurement.MeasureName}"",""{_measurement.Quantity}"",""{_measurement.Volume}"",""{_measurement.ABV}"",""{_measurement.Units}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var measurements = await _factory.BeverageConsumptionMeasurements.ListAsync(x => true, 1, int.MaxValue);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(_person.Id, measurements.First().PersonId);
            Assert.AreEqual(_beverage.Id, measurements.First().BeverageId);
            Assert.AreEqual(_measurement.Date, measurements.First().Date);
            Assert.AreEqual(_measurement.Measure, (int)measurements.First().Measure);
            Assert.AreEqual(_measurement.Quantity, measurements.First().Quantity);
            Assert.AreEqual(_measurement.ABV, measurements.First().ABV);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRecordFormatException))]
        public async Task InvalidRecordFormatTest()
        {
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", "Invalid record format"]);

            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var factory = new HealthTrackerFactory(context, null, null);
            var importer = new BeverageConsumptionMeasurementImporter(factory, ExportableBeverageConsumptionMeasurement.CsvRecordPattern);
            await importer.ImportAsync(_filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidBeverageTest()
        {
            var record = $@"""{_person.Id}"",""{_person.Name}"",""{_measurement.Date:dd-MMM-yyyy HH:mm:ss}"",""0"",""Not Valid"",""{(int)_measurement.Measure}"",""{_measurement.MeasureName}"",""{_measurement.Quantity}"",""{_measurement.Volume}"",""{_measurement.ABV}"",""{_measurement.Units}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidMeasureTest()
        {
            var record = $@"""{_person.Id}"",""{_person.Name}"",""{_measurement.Date:dd-MMM-yyyy HH:mm:ss}"",""{_beverage.Id}"",""{_beverage.Name}"",""0"","""",""{_measurement.Quantity}"",""{_measurement.Volume}"",""{_measurement.ABV}"",""{_measurement.Units}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidQuantityTest()
        {
            var record = $@"""{_person.Id}"",""{_person.Name}"",""{_measurement.Date:dd-MMM-yyyy HH:mm:ss}"",""{_beverage.Id}"",""{_beverage.Name}"",""{(int)_measurement.Measure}"",""{_measurement.MeasureName}"",""0"",""{_measurement.Volume}"",""{_measurement.ABV}"",""{_measurement.Units}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidVolumeTest()
        {
            var record = $@"""{_person.Id}"",""{_person.Name}"",""{_measurement.Date:dd-MMM-yyyy HH:mm:ss}"",""{_beverage.Id}"",""{_beverage.Name}"",""{(int)_measurement.Measure}"",""{_measurement.MeasureName}"",""{_measurement.Quantity}"",""0"",""{_measurement.ABV}"",""{_measurement.Units}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidABVTest()
        {
            var record = $@"""{_person.Id}"",""{_person.Name}"",""{_measurement.Date:dd-MMM-yyyy HH:mm:ss}"",""{_beverage.Id}"",""{_beverage.Name}"",""{(int)_measurement.Measure}"",""{_measurement.MeasureName}"",""{_measurement.Quantity}"",""{_measurement.Volume}"",""1000"",""{_measurement.Units}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);
        }
    }
}
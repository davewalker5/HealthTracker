using HealthTracker.Configuration.Entities;
using HealthTracker.Data;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Export;
using HealthTracker.DataExchange.Extensions;
using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Food;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.Alcohol
{
    [TestClass]
    public class BeverageConsumptionMeasurementExportTest
    {
        private readonly Person _person = DataGenerator.RandomPerson(16, 90);
        private readonly Beverage _beverage = DataGenerator.RandomBeverage();
        private BeverageConsumptionMeasurement _measurement;
        private string _filePath;

        private readonly HealthTrackerApplicationSettings _settings = new()
        {
            ShotSize = 25,
            SmallGlassSize = 125,
            MediumGlassSize = 175,
            LargeGlassSize = 250
        };

        [TestInitialize]
        public void Initialize()
        {
            _measurement = DataGenerator.RandomBeverageConsumptionMeasurement(_person.Id, _beverage.Id, 2024);
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
            var exportable = _measurement.ToExportable([_person], [_beverage]);
            Assert.AreEqual(_person.Id, exportable.PersonId);
            Assert.AreEqual(_person.Name, exportable.Name);
            Assert.AreEqual(_beverage.Id, exportable.BeverageId);
            Assert.AreEqual(_beverage.Name, exportable.Beverage);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_measurement.Quantity, exportable.Quantity);
            Assert.AreEqual(_measurement.Volume, exportable.Volume);
            Assert.AreEqual(_measurement.ABV, exportable.ABV);
        }

        [TestMethod]
        public void ConvertCollectionToExportable()
        {
            List<BeverageConsumptionMeasurement> measurements = [_measurement];
            var exportable = measurements.ToExportable([_person], [_beverage]);
            Assert.AreEqual(_person.Id, exportable.First().PersonId);
            Assert.AreEqual(_person.Name, exportable.First().Name);
            Assert.AreEqual(_beverage.Id, exportable.First().BeverageId);
            Assert.AreEqual(_beverage.Name, exportable.First().Beverage);
            Assert.AreEqual(_measurement.Date, exportable.First().Date);
            Assert.AreEqual(_measurement.Quantity, exportable.First().Quantity);
            Assert.AreEqual(_measurement.Volume, exportable.First().Volume);
            Assert.AreEqual(_measurement.ABV, exportable.First().ABV);
        }

        [TestMethod]
        public void FromCsvRecordTest()
        {
            var record = $@"""{_person.Id}"",""{_person.Name}"",""{_measurement.Date:dd-MMM-yyyy HH:mm:ss}"",""{_beverage.Id}"",""{_beverage.Name}"",""{_measurement.Quantity}"",""{_measurement.Volume}"",""{_measurement.ABV}"",""{_measurement.Units}""";
            var exportable = ExportableBeverageConsumptionMeasurement.FromCsv(record);
            Assert.AreEqual(_person.Id, exportable.PersonId);
            Assert.AreEqual($"{_person.Name}", exportable.Name);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_beverage.Id, exportable.BeverageId);
            Assert.AreEqual(_beverage.Name, exportable.Beverage);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_measurement.Quantity, exportable.Quantity);
            Assert.AreEqual(_measurement.Volume, exportable.Volume);
            Assert.AreEqual(_measurement.ABV, exportable.ABV);
        }

        [TestMethod]
        public async Task ExportAllMeasurementsTest()
        {
            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            await context.People.AddAsync(_person);
            await context.Beverages.AddAsync(_beverage);
            _measurement.BeverageId = _beverage.Id;
            await context.BeverageConsumptionMeasurements.AddAsync(_measurement);
            await context.SaveChangesAsync();

            var logger = new Mock<IHealthTrackerLogger>();
            var factory = new HealthTrackerFactory(context, _settings, logger.Object);
            var exporter = new BeverageConsumptionMeasurementExporter(factory);

            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            List<BeverageConsumptionMeasurement> measurements = [_measurement];
            await exporter.ExportAsync(measurements, _filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var records = File.ReadAllLines(_filePath);
            Assert.AreEqual(2, records.Length);

            var exportable = ExportableBeverageConsumptionMeasurement.FromCsv(records[1]);
            Assert.AreEqual(_person.Id, exportable.PersonId);
            Assert.AreEqual($"{_person.Name}", exportable.Name);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_beverage.Id, exportable.BeverageId);
            Assert.AreEqual(_beverage.Name, exportable.Beverage);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_measurement.Quantity, exportable.Quantity);
            Assert.AreEqual(_measurement.Volume, exportable.Volume);
            Assert.AreEqual(_measurement.ABV, exportable.ABV);
        }

        [TestMethod]
        public async Task ExportMeasurementsForPersonTest()
        {
            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            await context.People.AddAsync(_person);
            await context.Beverages.AddAsync(_beverage);
            _measurement.BeverageId = _beverage.Id;
            await context.BeverageConsumptionMeasurements.AddAsync(_measurement);
            await context.SaveChangesAsync();

            var logger = new Mock<IHealthTrackerLogger>();
            var factory = new HealthTrackerFactory(context, _settings, logger.Object);
            var exporter = new BeverageConsumptionMeasurementExporter(factory);

            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            await exporter.ExportAsync(_person.Id, null, null, _filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var records = File.ReadAllLines(_filePath);
            Assert.AreEqual(2, records.Length);

            var exportable = ExportableBeverageConsumptionMeasurement.FromCsv(records[1]);
            Assert.AreEqual(_person.Id, exportable.PersonId);
            Assert.AreEqual($"{_person.Name}", exportable.Name);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_beverage.Id, exportable.BeverageId);
            Assert.AreEqual(_beverage.Name, exportable.Beverage);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_measurement.Quantity, exportable.Quantity);
            Assert.AreEqual(_measurement.Volume, exportable.Volume);
            Assert.AreEqual(_measurement.ABV, exportable.ABV);
        }
    }
}
using HealthTracker.Data;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Export;
using HealthTracker.DataExchange.Extensions;
using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.Logic.Factory;
using Moq;
using HealthTracker.Tests.Mocks;

namespace HealthTracker.Tests.Weight
{
    [TestClass]
    public class WeightMeasurementExportTest
    {
        private readonly List<BMIBand> _bands = [
            new() { Name = "Underweight", MinimumBMI = 0, MaximumBMI = 18.49M, Order = 1 },
            new() { Name = "Normal", MinimumBMI = 18.5M, MaximumBMI = 24.9M, Order = 2 },
            new() { Name = "Overweight", MinimumBMI = 25M, MaximumBMI = 29.9M, Order = 3 },
            new() { Name = "Obese", MinimumBMI = 30M, MaximumBMI = 34.9M, Order = 4 },
            new() { Name = "Severely Obese", MinimumBMI = 35M, MaximumBMI = 39.9M, Order = 5 },
            new() { Name = "Morbidly Obese", MinimumBMI = 40M, MaximumBMI = 1000M, Order = 6 }
        ];

        private readonly Person _person = DataGenerator.RandomPerson(16, 90);
        private WeightMeasurement _measurement;
        private string _filePath;

        [TestInitialize]
        public async Task Initialise()
        {
            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            foreach (var band in _bands)
            {
                await context.BMIBands.AddAsync(band);
            }
            await context.People.AddAsync(_person);
            await context.SaveChangesAsync();
            var logger = new Mock<IHealthTrackerLogger>();
            var factory = new HealthTrackerFactory(context, null, logger.Object);

            _measurement = DataGenerator.RandomWeightMeasurement(_person.Id, 2024, 50, 100);
            await factory.WeightCalculator.CalculateRelatedProperties([_measurement]);
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
            Assert.AreEqual(_measurement.Weight, exportable.Weight);
            Assert.AreEqual(_measurement.BMI, exportable.BMI);
            Assert.AreEqual(_measurement.BMIAssessment, exportable.BMIAssessment);
        }

        [TestMethod]
        public void ConvertCollectionToExportable()
        {
            List<WeightMeasurement> measurements = [_measurement];
            var exportable = measurements.ToExportable([_person]);
            Assert.AreEqual(_person.Id, exportable.First().PersonId);
            Assert.AreEqual(_person.Name(), exportable.First().Name);
            Assert.AreEqual(_measurement.Date, exportable.First().Date);
            Assert.AreEqual(_measurement.Weight, exportable.First().Weight);
            Assert.AreEqual(_measurement.BMI, exportable.First().BMI);
            Assert.AreEqual(_measurement.BMIAssessment, exportable.First().BMIAssessment);
        }

        [TestMethod]
        public void FromCsvRecordTest()
        {
            var record = $@"""{_person.Id}"",""{_person.Name()}"",""{_measurement.Date:dd/MM/yyyy}"",""{_measurement.Weight}"",""{_measurement.BMI}"",""{_measurement.BMIAssessment}"",""{_measurement.BMR}""";
            var exportable = ExportableWeightMeasurement.FromCsv(record);
            Assert.AreEqual(_person.Id, exportable.PersonId);
            Assert.AreEqual(_person.Name(), exportable.Name);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_measurement.Weight, exportable.Weight);
            Assert.AreEqual(_measurement.BMI, exportable.BMI);
            Assert.AreEqual(_measurement.BMIAssessment, exportable.BMIAssessment);
        }

        [TestMethod]
        public async Task ExportAllMeasurementsTest()
        {
            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            await context.People.AddAsync(_person);
            await context.WeightMeasurements.AddAsync(_measurement);
            await context.SaveChangesAsync();

            var logger = new Mock<IHealthTrackerLogger>();
            var factory = new HealthTrackerFactory(context, null, logger.Object);
            var exporter = new WeightMeasurementExporter(factory);

            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            List<WeightMeasurement> measurements = [_measurement];
            await exporter.ExportAsync(measurements, _filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var records = File.ReadAllLines(_filePath);
            Assert.AreEqual(2, records.Length);

            var exportable = ExportableWeightMeasurement.FromCsv(records[1]);
            Assert.AreEqual(_person.Id, exportable.PersonId);
            Assert.AreEqual(_person.Name(), exportable.Name);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_measurement.Weight, exportable.Weight);
        }

        [TestMethod]
        public async Task ExportMeasurementsForPersonTest()
        {
            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            await context.People.AddAsync(_person);
            await context.WeightMeasurements.AddAsync(_measurement);
            await context.SaveChangesAsync();

            var logger = new Mock<IHealthTrackerLogger>();
            var factory = new HealthTrackerFactory(context, null, logger.Object);
            var exporter = new WeightMeasurementExporter(factory);

            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            await exporter.ExportAsync(_person.Id, null, null, _filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var records = File.ReadAllLines(_filePath);
            Assert.AreEqual(2, records.Length);

            var exportable = ExportableWeightMeasurement.FromCsv(records[1]);
            Assert.AreEqual(_person.Id, exportable.PersonId);
            Assert.AreEqual(_person.Name(), exportable.Name);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_measurement.Weight, exportable.Weight);
        }
    }
}
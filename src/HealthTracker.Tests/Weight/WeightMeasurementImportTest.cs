using HealthTracker.Data;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Exceptions;
using HealthTracker.DataExchange.Extensions;
using HealthTracker.DataExchange.Import;
using HealthTracker.DataExchange.Interfaces;
using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.Weight
{
    [TestClass]
    public class WeightMeasurementImportTest
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
        private ExportableWeightMeasurement _measurement;
        private string _filePath;
        private IHealthTrackerFactory _factory;
        private IWeightMeasurementImporter _importer;

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
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            _importer = new WeightMeasurementImporter(_factory, ExportableWeightMeasurement.CsvRecordPattern);

            var measurement = DataGenerator.RandomWeightMeasurement(_person.Id, 2024, 50, 100);
            await _factory.WeightCalculator.CalculateRelatedProperties([measurement]);
            _measurement = measurement.ToExportable([_person]);
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
            Assert.AreEqual(_measurement.Weight, measurement.Weight);
        }

        [TestMethod]
        public void ConvertCollectionFromExportable()
        {
            List<ExportableWeightMeasurement> exportable = [_measurement];
            var measurements = exportable.FromExportable();
            Assert.AreEqual(_measurement.PersonId, measurements.First().PersonId);
            Assert.AreEqual(_measurement.Date, measurements.First().Date);
            Assert.AreEqual(_measurement.Weight, measurements.First().Weight);
        }

        [TestMethod]
        public async Task ImportMeasurementsTest()
        {
            var record = $@"""{_measurement.PersonId}"",""{_person.Name()}"",""{_measurement.Date:dd/MM/yyyy}"",""{_measurement.Weight}"",""{_measurement.BMI}"",""{_measurement.BMIAssessment}"",""{_measurement.BMR}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);
            await _importer.ImportAsync(_filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var measurements = await _factory.WeightMeasurements.ListAsync(x => true);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(_measurement.PersonId, measurements.First().PersonId);
            Assert.AreEqual(_measurement.Date, measurements.First().Date);
            Assert.AreEqual(_measurement.Weight, measurements.First().Weight);

            File.Delete(_filePath);
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
        public async Task InvalidWeightTest()
        {
            var record = $@"""{_measurement.PersonId}"",""{_person.Name()}"",""{_measurement.Date:dd/MM/yyyy}"",""{0}"",""{_measurement.BMI}"",""{_measurement.BMIAssessment}"",""{_measurement.BMR}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);
        }
    }
}
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

namespace HealthTracker.Tests.Exercise
{
    [TestClass]
    public class ExerciseMeasurementImportTest
    {
        private readonly Person _person = DataGenerator.RandomPerson(16, 90);
        private readonly ActivityType _activityType = DataGenerator.RandomActivityType();
        private ExportableExerciseMeasurement _measurement;
        private string _filePath;

        private IHealthTrackerFactory _factory;
        private IExerciseMeasurementImporter _importer;

        [TestInitialize]
        public async Task Initialise()
        {
            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            await context.People.AddAsync(_person);
            await context.ActivityTypes.AddAsync(_activityType);
            await context.SaveChangesAsync();

            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            _importer = new ExerciseMeasurementImporter(_factory, ExportableExerciseMeasurement.CsvRecordPattern);

            _measurement = DataGenerator.RandomExerciseMeasurement(_person.Id, _activityType.Id, DataGenerator.RandomDateInYear(2024)).ToExportable([_person], [_activityType]);
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
            var measurement = _measurement.FromExportable([_activityType]);
            Assert.AreEqual(_person.Id, measurement.PersonId);
            Assert.AreEqual(_activityType.Id, measurement.ActivityTypeId);
            Assert.AreEqual(_measurement.Date, measurement.Date);
            Assert.AreEqual(_measurement.Duration.ToDuration(), measurement.Duration);
            Assert.AreEqual(_measurement.Distance, measurement.Distance);
            Assert.AreEqual(_measurement.Calories, measurement.Calories);
            Assert.AreEqual(_measurement.MinimumHeartRate, measurement.MinimumHeartRate);
            Assert.AreEqual(_measurement.MaximumHeartRate, measurement.MaximumHeartRate);
        }

        [TestMethod]
        public void ConvertCollectionFromExportable()
        {
            List<ExportableExerciseMeasurement> exportable = [_measurement];
            var measurements = exportable.FromExportable([_activityType]);
            Assert.AreEqual(_person.Id, measurements.First().PersonId);
            Assert.AreEqual(_activityType.Id, measurements.First().ActivityTypeId);
            Assert.AreEqual(_measurement.Date, measurements.First().Date);
            Assert.AreEqual(_measurement.Duration.ToDuration(), measurements.First().Duration);
            Assert.AreEqual(_measurement.Distance, measurements.First().Distance);
            Assert.AreEqual(_measurement.Calories, measurements.First().Calories);
            Assert.AreEqual(_measurement.MinimumHeartRate, measurements.First().MinimumHeartRate);
            Assert.AreEqual(_measurement.MaximumHeartRate, measurements.First().MaximumHeartRate);
        }

        [TestMethod]
        public async Task ImportMeasurementsTest()
        {
            var record = $@"""{_person.Id}"",""{_person.Name()}"",""{_measurement.Date:dd/MM/yyyy}"",""{_activityType.Description}"",""{_measurement.Duration}"",""{_measurement.Distance}"",""{_measurement.Calories}"",""{_measurement.MinimumHeartRate}"",""{_measurement.MaximumHeartRate}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var measurements = await _factory.ExerciseMeasurements.ListAsync(x => true);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(_person.Id, measurements.First().PersonId);
            Assert.AreEqual(_activityType.Id, measurements.First().ActivityTypeId);
            Assert.AreEqual(_measurement.Date, measurements.First().Date);
            Assert.AreEqual(_measurement.Duration.ToDuration(), measurements.First().Duration);
            Assert.AreEqual(_measurement.Distance, measurements.First().Distance);
            Assert.AreEqual(_measurement.Calories, measurements.First().Calories);
            Assert.AreEqual(_measurement.MinimumHeartRate, measurements.First().MinimumHeartRate);
            Assert.AreEqual(_measurement.MaximumHeartRate, measurements.First().MaximumHeartRate);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRecordFormatException))]
        public async Task InvalidRecordFormatTest()
        {
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", "Invalid record format"]);

            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var factory = new HealthTrackerFactory(context, null, null);
            var importer = new ExerciseMeasurementImporter(factory, ExportableExerciseMeasurement.CsvRecordPattern);
            await importer.ImportAsync(_filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidActivityTypeTest()
        {
            var record = $@"""{_person.Id}"",""{_person.Name()}"",""{_measurement.Date:dd/MM/yyyy}"",""Not valid"",""{_measurement.Duration}"",""{_measurement.Distance}"",""{_measurement.Calories}"",""{_measurement.MinimumHeartRate}"",""{_measurement.MaximumHeartRate}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidDurationTest()
        {
            var record = $@"""{_person.Id}"",""{_person.Name()}"",""{_measurement.Date:dd/MM/yyyy}"",""{_activityType.Description}"",""00:00:00"",""{_measurement.Distance}"",""{_measurement.Calories}"",""{_measurement.MinimumHeartRate}"",""{_measurement.MaximumHeartRate}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidHeartRateTest()
        {
            var record = $@"""{_person.Id}"",""{_person.Name()}"",""{_measurement.Date:dd/MM/yyyy}"",""{_activityType.Description}"",""{_measurement.Duration}"",""{_measurement.Distance}"",""{_measurement.Calories}"",""{_measurement.MaximumHeartRate}"",""{_measurement.MinimumHeartRate}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);
        }
    }
}
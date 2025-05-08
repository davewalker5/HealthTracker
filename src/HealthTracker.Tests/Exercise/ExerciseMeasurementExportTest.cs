using HealthTracker.Data;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Export;
using HealthTracker.DataExchange.Extensions;
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
    public class ExerciseMeasurementExportTest
    {
        private readonly Person _person = DataGenerator.RandomPerson(16, 90);
        private readonly ActivityType _activityType = DataGenerator.RandomActivityType();
        private ExerciseMeasurement _measurement;
        private string _filePath;

        [TestInitialize]
        public void Initialize()
        {
            _measurement = DataGenerator.RandomExerciseMeasurement(_person.Id, _activityType.Id, DataGenerator.RandomDateInYear(2024));
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
            var exportable = _measurement.ToExportable([_person], [_activityType]);
            Assert.AreEqual(_person.Id, exportable.PersonId);
            Assert.AreEqual(_person.Name, exportable.Name);
            Assert.AreEqual(_activityType.Description, exportable.ActivityType);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_measurement.Duration.ToFormattedDuration(), exportable.Duration);
            Assert.AreEqual(_measurement.Distance, exportable.Distance);
            Assert.AreEqual(_measurement.Calories, exportable.Calories);
            Assert.AreEqual(_measurement.MinimumHeartRate, exportable.MinimumHeartRate);
            Assert.AreEqual(_measurement.MaximumHeartRate, exportable.MaximumHeartRate);
        }

        [TestMethod]
        public void ConvertCollectionToExportable()
        {
            List<ExerciseMeasurement> measurements = [_measurement];
            var exportable = measurements.ToExportable([_person], [_activityType]);
            Assert.AreEqual(_person.Id, exportable.First().PersonId);
            Assert.AreEqual(_person.Name, exportable.First().Name);
            Assert.AreEqual(_activityType.Description, exportable.First().ActivityType);
            Assert.AreEqual(_measurement.Date, exportable.First().Date);
            Assert.AreEqual(_measurement.Duration.ToFormattedDuration(), exportable.First().Duration);
            Assert.AreEqual(_measurement.Distance, exportable.First().Distance);
            Assert.AreEqual(_measurement.Calories, exportable.First().Calories);
            Assert.AreEqual(_measurement.MinimumHeartRate, exportable.First().MinimumHeartRate);
            Assert.AreEqual(_measurement.MaximumHeartRate, exportable.First().MaximumHeartRate);
        }

        [TestMethod]
        public void FromCsvRecordTest()
        {
            var record = $@"""{_person.Id}"",""{_person.Name}"",""{_measurement.Date:dd/MM/yyyy}"",""{_activityType.Description}"",""{_measurement.Duration.ToFormattedDuration()}"",""{_measurement.Distance}"",""{_measurement.Calories}"",""{_measurement.MinimumHeartRate}"",""{_measurement.MaximumHeartRate}""";
            var exportable = ExportableExerciseMeasurement.FromCsv(record);
            Assert.AreEqual(_person.Id, exportable.PersonId);
            Assert.AreEqual($"{_person.Name}", exportable.Name);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_activityType.Description, exportable.ActivityType);
            Assert.AreEqual(_measurement.Duration.ToFormattedDuration(), exportable.Duration);
            Assert.AreEqual(_measurement.Distance, exportable.Distance);
            Assert.AreEqual(_measurement.Calories, exportable.Calories);
            Assert.AreEqual(_measurement.MinimumHeartRate, exportable.MinimumHeartRate);
            Assert.AreEqual(_measurement.MaximumHeartRate, exportable.MaximumHeartRate);
        }

        [TestMethod]
        public async Task ExportAllMeasurementsTest()
        {
            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            await context.People.AddAsync(_person);
            await context.ActivityTypes.AddAsync(_activityType);
            await context.ExerciseMeasurements.AddAsync(_measurement);
            await context.SaveChangesAsync();

            var logger = new Mock<IHealthTrackerLogger>();
            var factory = new HealthTrackerFactory(context, null, logger.Object);
            var exporter = new ExerciseMeasurementExporter(factory);

            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            List<ExerciseMeasurement> measurements = [_measurement];
            await exporter.ExportAsync(measurements, _filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var records = File.ReadAllLines(_filePath);
            Assert.AreEqual(2, records.Length);

            var exportable = ExportableExerciseMeasurement.FromCsv(records[1]);
            Assert.AreEqual(_person.Id, exportable.PersonId);
            Assert.AreEqual($"{_person.Name}", exportable.Name);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_activityType.Description, exportable.ActivityType);
            Assert.AreEqual(_measurement.Duration.ToFormattedDuration(), exportable.Duration);
            Assert.AreEqual(_measurement.Distance, exportable.Distance);
            Assert.AreEqual(_measurement.Calories, exportable.Calories);
            Assert.AreEqual(_measurement.MinimumHeartRate, exportable.MinimumHeartRate);
            Assert.AreEqual(_measurement.MaximumHeartRate, exportable.MaximumHeartRate);
        }

        [TestMethod]
        public async Task ExportMeasurementsForPersonTest()
        {
            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            await context.People.AddAsync(_person);
            await context.ActivityTypes.AddAsync(_activityType);
            await context.ExerciseMeasurements.AddAsync(_measurement);
            await context.SaveChangesAsync();

            var logger = new Mock<IHealthTrackerLogger>();
            var factory = new HealthTrackerFactory(context, null, logger.Object);
            var exporter = new ExerciseMeasurementExporter(factory);

            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            await exporter.ExportAsync(_person.Id, null, null, _filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var records = File.ReadAllLines(_filePath);
            Assert.AreEqual(2, records.Length);

            var exportable = ExportableExerciseMeasurement.FromCsv(records[1]);
            Assert.AreEqual(_person.Id, exportable.PersonId);
            Assert.AreEqual($"{_person.Name}", exportable.Name);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_activityType.Description, exportable.ActivityType);
            Assert.AreEqual(_measurement.Duration.ToFormattedDuration(), exportable.Duration);
            Assert.AreEqual(_measurement.Distance, exportable.Distance);
            Assert.AreEqual(_measurement.Calories, exportable.Calories);
            Assert.AreEqual(_measurement.MinimumHeartRate, exportable.MinimumHeartRate);
            Assert.AreEqual(_measurement.MaximumHeartRate, exportable.MaximumHeartRate);
        }
    }
}
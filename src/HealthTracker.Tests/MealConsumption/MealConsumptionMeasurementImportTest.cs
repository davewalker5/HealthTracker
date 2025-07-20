using HealthTracker.Data;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Exceptions;
using HealthTracker.DataExchange.Extensions;
using HealthTracker.DataExchange.Import;
using HealthTracker.DataExchange.Interfaces;
using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Food;
using HealthTracker.Logic.Extensions;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.MealConsumption
{
    [TestClass]
    public class MealConsumptionMeasurementImportTest
    {
        private readonly Person _person = DataGenerator.RandomPerson(16, 90);
        private MealConsumptionMeasurement _measurement;
        private ExportableMealConsumptionMeasurement _exportable;
        private string _filePath;

        private IHealthTrackerFactory _factory;
        private IMealConsumptionMeasurementImporter _importer;

        [TestInitialize]
        public async Task Initialise()
        {
            _measurement = DataGenerator.RandomMealConsumptionMeasurement(_person.Id, 2024);

            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            await context.People.AddAsync(_person);
            await context.Meals.AddAsync(_measurement.Meal);
            await context.NutritionalValues.AddAsync(_measurement.NutritionalValue);
            await context.SaveChangesAsync();

            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            _importer = new MealConsumptionMeasurementImporter(_factory, ExportableMealConsumptionMeasurement.CsvRecordPattern);

            _exportable = _measurement.ToExportable([_person], [_measurement.Meal]);
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
            var measurement = _exportable.FromExportable([_measurement.Meal]);
            Assert.AreEqual(_person.Id, measurement.PersonId);
            Assert.AreEqual(_exportable.MealId, measurement.MealId);
            Assert.AreEqual(_exportable.Date, measurement.Date);
            Assert.AreEqual(_exportable.Quantity, measurement.Quantity);
            Assert.AreEqual(_exportable.Calories, measurement.NutritionalValue.Calories);
            Assert.AreEqual(_exportable.Fat, measurement.NutritionalValue.Fat);
            Assert.AreEqual(_exportable.SaturatedFat, measurement.NutritionalValue.SaturatedFat);
            Assert.AreEqual(_exportable.Protein, measurement.NutritionalValue.Protein);
            Assert.AreEqual(_exportable.Carbohydrates, measurement.NutritionalValue.Carbohydrates);
            Assert.AreEqual(_exportable.Sugar, measurement.NutritionalValue.Sugar);
            Assert.AreEqual(_exportable.Fibre, measurement.NutritionalValue.Fibre);
        }

        [TestMethod]
        public void ConvertCollectionFromExportable()
        {
            List<ExportableMealConsumptionMeasurement> exportable = [_exportable];
            var measurements = exportable.FromExportable([_measurement.Meal]);
            Assert.AreEqual(_person.Id, measurements.First().PersonId);
            Assert.AreEqual(_exportable.MealId, measurements.First().MealId);
            Assert.AreEqual(_exportable.Date, measurements.First().Date);
            Assert.AreEqual(_exportable.Quantity, measurements.First().Quantity);
            Assert.AreEqual(_exportable.Calories, measurements.First().NutritionalValue.Calories);
            Assert.AreEqual(_exportable.Fat, measurements.First().NutritionalValue.Fat);
            Assert.AreEqual(_exportable.SaturatedFat, measurements.First().NutritionalValue.SaturatedFat);
            Assert.AreEqual(_exportable.Protein, measurements.First().NutritionalValue.Protein);
            Assert.AreEqual(_exportable.Carbohydrates, measurements.First().NutritionalValue.Carbohydrates);
            Assert.AreEqual(_exportable.Sugar, measurements.First().NutritionalValue.Sugar);
            Assert.AreEqual(_exportable.Fibre, measurements.First().NutritionalValue.Fibre);
        }

        [TestMethod]
        public async Task ImportMeasurementsTest()
        {
            var record = $@"""{_person.Id}"",""{_person.Name}"",""{_measurement.Date:dd-MMM-yyyy HH:mm:ss}"",""{_measurement.MealId}"",""{_measurement.Meal.Name}"",""{_measurement.Quantity}"",""{_measurement.NutritionalValue.Calories}"",""{_measurement.NutritionalValue.Fat}"",""{_measurement.NutritionalValue.SaturatedFat}"",""{_measurement.NutritionalValue.Protein}"",""{_measurement.NutritionalValue.Carbohydrates}"",""{_measurement.NutritionalValue.Sugar}"",""{_measurement.NutritionalValue.Fibre}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var measurements = await _factory.MealConsumptionMeasurements.ListAsync(x => true, 1, int.MaxValue);
            Assert.AreEqual(1, measurements.Count);
            Assert.AreEqual(_person.Id, measurements.First().PersonId);
            Assert.AreEqual(_exportable.MealId, measurements.First().MealId);
            Assert.AreEqual(_exportable.Date, measurements.First().Date);
            Assert.AreEqual(_exportable.Quantity, measurements.First().Quantity);
            Assert.AreEqual(_exportable.Calories, measurements.First().NutritionalValue.Calories);
            Assert.AreEqual(_exportable.Fat, measurements.First().NutritionalValue.Fat);
            Assert.AreEqual(_exportable.SaturatedFat, measurements.First().NutritionalValue.SaturatedFat);
            Assert.AreEqual(_exportable.Protein, measurements.First().NutritionalValue.Protein);
            Assert.AreEqual(_exportable.Carbohydrates, measurements.First().NutritionalValue.Carbohydrates);
            Assert.AreEqual(_exportable.Sugar, measurements.First().NutritionalValue.Sugar);
            Assert.AreEqual(_exportable.Fibre, measurements.First().NutritionalValue.Fibre);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRecordFormatException))]
        public async Task InvalidRecordFormatTest()
        {
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", "Invalid record format"]);

            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var factory = new HealthTrackerFactory(context, null, null);
            var importer = new MealConsumptionMeasurementImporter(factory, ExportableMealConsumptionMeasurement.CsvRecordPattern);
            await importer.ImportAsync(_filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidMealTest()
        {
            var record = $@"""{_person.Id}"",""{_person.Name}"",""{_measurement.Date:dd-MMM-yyyy HH:mm:ss}"",""0"",""Not Valid"",""{_measurement.Quantity}"",""{_measurement.NutritionalValue.Calories}"",""{_measurement.NutritionalValue.Fat}"",""{_measurement.NutritionalValue.SaturatedFat}"",""{_measurement.NutritionalValue.Protein}"",""{_measurement.NutritionalValue.Carbohydrates}"",""{_measurement.NutritionalValue.Sugar}"",""{_measurement.NutritionalValue.Fibre}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidQuantityTest()
        {
            var record = $@"""{_person.Id}"",""{_person.Name}"",""{_measurement.Date:dd-MMM-yyyy HH:mm:ss}"",""{_measurement.MealId}"",""{_measurement.Meal.Name}"",""0"",""{_measurement.NutritionalValue.Calories}"",""{_measurement.NutritionalValue.Fat}"",""{_measurement.NutritionalValue.SaturatedFat}"",""{_measurement.NutritionalValue.Protein}"",""{_measurement.NutritionalValue.Carbohydrates}"",""{_measurement.NutritionalValue.Sugar}"",""{_measurement.NutritionalValue.Fibre}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);
        }
    }
}
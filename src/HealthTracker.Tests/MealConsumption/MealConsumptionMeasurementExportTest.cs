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

namespace HealthTracker.Tests.MealConsumption
{
    [TestClass]
    public class MealConsumptionMeasurementExportTest
    {
        private readonly Person _person = DataGenerator.RandomPerson(16, 90);
        private MealConsumptionMeasurement _measurement;
        private string _filePath;

        [TestInitialize]
        public void Initialize()
        {
            _measurement = DataGenerator.RandomMealConsumptionMeasurement(_person.Id, 2024);
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
            var exportable = _measurement.ToExportable([_person], [_measurement.Meal]);
            Assert.AreEqual(_person.Id, exportable.PersonId);
            Assert.AreEqual(_person.Name, exportable.Name);
            Assert.AreEqual(_measurement.Meal.Id, exportable.MealId);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_measurement.Quantity, exportable.Quantity);
            Assert.AreEqual(_measurement.NutritionalValue.Calories, exportable.Calories);
            Assert.AreEqual(_measurement.NutritionalValue.Fat, exportable.Fat);
            Assert.AreEqual(_measurement.NutritionalValue.SaturatedFat, exportable.SaturatedFat);
            Assert.AreEqual(_measurement.NutritionalValue.Protein, exportable.Protein);
            Assert.AreEqual(_measurement.NutritionalValue.Carbohydrates, exportable.Carbohydrates);
            Assert.AreEqual(_measurement.NutritionalValue.Sugar, exportable.Sugar);
            Assert.AreEqual(_measurement.NutritionalValue.Fibre, exportable.Fibre);
        }

        [TestMethod]
        public void ConvertCollectionToExportable()
        {
            List<MealConsumptionMeasurement> measurements = [_measurement];
            var exportable = measurements.ToExportable([_person], [_measurement.Meal]);
            Assert.AreEqual(_person.Id, exportable.First().PersonId);
            Assert.AreEqual(_person.Name, exportable.First().Name);
            Assert.AreEqual(_measurement.Meal.Id, exportable.First().MealId);
            Assert.AreEqual(_measurement.Date, exportable.First().Date);
            Assert.AreEqual(_measurement.Quantity, exportable.First().Quantity);
            Assert.AreEqual(_measurement.NutritionalValue.Calories, exportable.First().Calories);
            Assert.AreEqual(_measurement.NutritionalValue.Fat, exportable.First().Fat);
            Assert.AreEqual(_measurement.NutritionalValue.SaturatedFat, exportable.First().SaturatedFat);
            Assert.AreEqual(_measurement.NutritionalValue.Protein, exportable.First().Protein);
            Assert.AreEqual(_measurement.NutritionalValue.Carbohydrates, exportable.First().Carbohydrates);
            Assert.AreEqual(_measurement.NutritionalValue.Sugar, exportable.First().Sugar);
            Assert.AreEqual(_measurement.NutritionalValue.Fibre, exportable.First().Fibre);
        }

        [TestMethod]
        public void FromCsvRecordTest()
        {
            var record = $@"""{_person.Id}"",""{_person.Name}"",""{_measurement.Date:dd-MMM-yyyy HH:mm:ss}"",""{_measurement.MealId}"",""{_measurement.Meal.Name}"",""{_measurement.Quantity}"",""{_measurement.NutritionalValue.Calories}"",""{_measurement.NutritionalValue.Fat}"",""{_measurement.NutritionalValue.SaturatedFat}"",""{_measurement.NutritionalValue.Protein}"",""{_measurement.NutritionalValue.Carbohydrates}"",""{_measurement.NutritionalValue.Sugar}"",""{_measurement.NutritionalValue.Fibre}""";
            var exportable = ExportableMealConsumptionMeasurement.FromCsv(record);
            Assert.AreEqual(_person.Id, exportable.PersonId);
            Assert.AreEqual($"{_person.Name}", exportable.Name);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_measurement.Meal.Id, exportable.MealId);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_measurement.Quantity, exportable.Quantity);
            Assert.AreEqual(_measurement.NutritionalValue.Calories, exportable.Calories);
            Assert.AreEqual(_measurement.NutritionalValue.Fat, exportable.Fat);
            Assert.AreEqual(_measurement.NutritionalValue.SaturatedFat, exportable.SaturatedFat);
            Assert.AreEqual(_measurement.NutritionalValue.Protein, exportable.Protein);
            Assert.AreEqual(_measurement.NutritionalValue.Carbohydrates, exportable.Carbohydrates);
            Assert.AreEqual(_measurement.NutritionalValue.Sugar, exportable.Sugar);
            Assert.AreEqual(_measurement.NutritionalValue.Fibre, exportable.Fibre);
        }

        [TestMethod]
        public async Task ExportAllMeasurementsTest()
        {
            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            await context.People.AddAsync(_person);
            await context.Meals.AddAsync(_measurement.Meal);
            await context.NutritionalValues.AddAsync(_measurement.NutritionalValue);
            await context.MealConsumptionMeasurements.AddAsync(_measurement);
            await context.SaveChangesAsync();

            var logger = new Mock<IHealthTrackerLogger>();
            var factory = new HealthTrackerFactory(context, null, logger.Object);
            var exporter = new MealConsumptionMeasurementExporter(factory);

            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            List<MealConsumptionMeasurement> measurements = [_measurement];
            await exporter.ExportAsync(measurements, _filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var records = File.ReadAllLines(_filePath);
            Assert.AreEqual(2, records.Length);

            var exportable = ExportableMealConsumptionMeasurement.FromCsv(records[1]);
            Assert.AreEqual(_person.Id, exportable.PersonId);
            Assert.AreEqual($"{_person.Name}", exportable.Name);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_measurement.Meal.Id, exportable.MealId);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_measurement.Quantity, exportable.Quantity);
            Assert.AreEqual(_measurement.NutritionalValue.Calories, exportable.Calories);
            Assert.AreEqual(_measurement.NutritionalValue.Fat, exportable.Fat);
            Assert.AreEqual(_measurement.NutritionalValue.SaturatedFat, exportable.SaturatedFat);
            Assert.AreEqual(_measurement.NutritionalValue.Protein, exportable.Protein);
            Assert.AreEqual(_measurement.NutritionalValue.Carbohydrates, exportable.Carbohydrates);
            Assert.AreEqual(_measurement.NutritionalValue.Sugar, exportable.Sugar);
            Assert.AreEqual(_measurement.NutritionalValue.Fibre, exportable.Fibre);
        }

        [TestMethod]
        public async Task ExportMeasurementsForPersonTest()
        {
            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            await context.People.AddAsync(_person);
            await context.Meals.AddAsync(_measurement.Meal);
            await context.NutritionalValues.AddAsync(_measurement.NutritionalValue);
            await context.MealConsumptionMeasurements.AddAsync(_measurement);
            await context.SaveChangesAsync();

            var logger = new Mock<IHealthTrackerLogger>();
            var factory = new HealthTrackerFactory(context, null, logger.Object);
            var exporter = new MealConsumptionMeasurementExporter(factory);

            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            await exporter.ExportAsync(_person.Id, null, null, _filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var records = File.ReadAllLines(_filePath);
            Assert.AreEqual(2, records.Length);

            var exportable = ExportableMealConsumptionMeasurement.FromCsv(records[1]);
            Assert.AreEqual(_person.Id, exportable.PersonId);
            Assert.AreEqual($"{_person.Name}", exportable.Name);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_measurement.Meal.Id, exportable.MealId);
            Assert.AreEqual(_measurement.Date, exportable.Date);
            Assert.AreEqual(_measurement.Quantity, exportable.Quantity);
            Assert.AreEqual(_measurement.NutritionalValue.Calories, exportable.Calories);
            Assert.AreEqual(_measurement.NutritionalValue.Fat, exportable.Fat);
            Assert.AreEqual(_measurement.NutritionalValue.SaturatedFat, exportable.SaturatedFat);
            Assert.AreEqual(_measurement.NutritionalValue.Protein, exportable.Protein);
            Assert.AreEqual(_measurement.NutritionalValue.Carbohydrates, exportable.Carbohydrates);
            Assert.AreEqual(_measurement.NutritionalValue.Sugar, exportable.Sugar);
            Assert.AreEqual(_measurement.NutritionalValue.Fibre, exportable.Fibre);
        }
    }
}
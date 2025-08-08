using HealthTracker.Data;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Exceptions;
using HealthTracker.DataExchange.Extensions;
using HealthTracker.DataExchange.Import;
using HealthTracker.DataExchange.Interfaces;
using HealthTracker.Entities.Food;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.Meals
{
    [TestClass]
    public class MealImportTest
    {
        private FoodSource _source;
        private ExportableMeal _exportable;
        private string _filePath;

        private IHealthTrackerFactory _factory;
        private IMealImporter _importer;

        [TestInitialize]
        public async Task Initialise()
        {
            var meal = DataGenerator.RandomMeal(0);

            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            await context.FoodSources.AddAsync(meal.FoodSource);
            await context.NutritionalValues.AddAsync(meal.NutritionalValue);
            await context.SaveChangesAsync();

            meal.FoodSourceId = meal.FoodSource.Id;
            meal.NutritionalValueId = meal.NutritionalValue.Id;

            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            _importer = new MealImporter(_factory, ExportableMeal.CsvRecordPattern);

            _source = meal.FoodSource;
            _exportable = meal.ToExportable();
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
            var meal = _exportable.FromExportable([_source]);
            Assert.AreEqual(_exportable.Name, meal.Name);
            Assert.AreEqual(_exportable.Portions, meal.Portions);
            Assert.AreEqual(_exportable.FoodSource, meal.FoodSource.Name);
            Assert.AreEqual(_exportable.Reference, meal.Reference);
            Assert.AreEqual(_exportable.Calories, meal.NutritionalValue.Calories);
            Assert.AreEqual(_exportable.Fat, meal.NutritionalValue.Fat);
            Assert.AreEqual(_exportable.SaturatedFat, meal.NutritionalValue.SaturatedFat);
            Assert.AreEqual(_exportable.Protein, meal.NutritionalValue.Protein);
            Assert.AreEqual(_exportable.Carbohydrates, meal.NutritionalValue.Carbohydrates);
            Assert.AreEqual(_exportable.Sugar, meal.NutritionalValue.Sugar);
            Assert.AreEqual(_exportable.Fibre, meal.NutritionalValue.Fibre);
        }

        [TestMethod]
        public void ConvertCollectionFromExportable()
        {
            List<ExportableMeal> exportable = [_exportable];
            var meals = exportable.FromExportable([_source]);
            Assert.AreEqual(_exportable.Name, meals.First().Name);
            Assert.AreEqual(_exportable.Portions, meals.First().Portions);
            Assert.AreEqual(_exportable.FoodSource, meals.First().FoodSource.Name);
            Assert.AreEqual(_exportable.Reference, meals.First().Reference);
            Assert.AreEqual(_exportable.Calories, meals.First().NutritionalValue.Calories);
            Assert.AreEqual(_exportable.Fat, meals.First().NutritionalValue.Fat);
            Assert.AreEqual(_exportable.SaturatedFat, meals.First().NutritionalValue.SaturatedFat);
            Assert.AreEqual(_exportable.Protein, meals.First().NutritionalValue.Protein);
            Assert.AreEqual(_exportable.Carbohydrates, meals.First().NutritionalValue.Carbohydrates);
            Assert.AreEqual(_exportable.Sugar, meals.First().NutritionalValue.Sugar);
            Assert.AreEqual(_exportable.Fibre, meals.First().NutritionalValue.Fibre);
        }

        [TestMethod]
        public async Task ImportTest()
        {
            var record = $@"""{_exportable.Name}"",""{_exportable.FoodSource}"",""{_exportable.Reference}"",""{_exportable.Portions}"",""{_exportable.Calories}"",""{_exportable.Fat}"",""{_exportable.SaturatedFat}"",""{_exportable.Protein}"",""{_exportable.Carbohydrates}"",""{_exportable.Sugar}"",""{_exportable.Fibre}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var meals = await _factory.Meals.ListAsync(x => true, 1, int.MaxValue);
            Assert.AreEqual(1, meals.Count);
            Assert.AreEqual(_exportable.Name, meals.First().Name);
            Assert.AreEqual(_exportable.Portions, meals.First().Portions);
            Assert.AreEqual(_exportable.FoodSource, meals.First().FoodSource.Name);
            Assert.AreEqual(_exportable.Calories, meals.First().NutritionalValue.Calories);
            Assert.AreEqual(_exportable.Fat, meals.First().NutritionalValue.Fat);
            Assert.AreEqual(_exportable.SaturatedFat, meals.First().NutritionalValue.SaturatedFat);
            Assert.AreEqual(_exportable.Protein, meals.First().NutritionalValue.Protein);
            Assert.AreEqual(_exportable.Carbohydrates, meals.First().NutritionalValue.Carbohydrates);
            Assert.AreEqual(_exportable.Sugar, meals.First().NutritionalValue.Sugar);
            Assert.AreEqual(_exportable.Fibre, meals.First().NutritionalValue.Fibre);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRecordFormatException))]
        public async Task InvalidRecordFormatTest()
        {
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", "Invalid record format"]);

            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var factory = new HealthTrackerFactory(context, null, null);
            var importer = new MealImporter(factory, ExportableMeal.CsvRecordPattern);
            await importer.ImportAsync(_filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidNameTest()
        {
            var record = $@""""",""{_exportable.FoodSource}"",""{_exportable.Reference}"",""{_exportable.Portions}"",""{_exportable.Calories}"",""{_exportable.Fat}"",""{_exportable.SaturatedFat}"",""{_exportable.Protein}"",""{_exportable.Carbohydrates}"",""{_exportable.Sugar}"",""{_exportable.Fibre}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidFoodSourceTest()
        {
            var record = $@"""{_exportable.Name}"","""",""{_exportable.Reference}"",""{_exportable.Portions}"",""{_exportable.Calories}"",""{_exportable.Fat}"",""{_exportable.SaturatedFat}"",""{_exportable.Protein}"",""{_exportable.Carbohydrates}"",""{_exportable.Sugar}"",""{_exportable.Fibre}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidPortionsTest()
        {
            var record = $@"""{_exportable.Name}"",""{_exportable.FoodSource}"",""{_exportable.Reference}"",""0"",""{_exportable.Calories}"",""{_exportable.Fat}"",""{_exportable.SaturatedFat}"",""{_exportable.Protein}"",""{_exportable.Carbohydrates}"",""{_exportable.Sugar}"",""{_exportable.Fibre}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);
        }
    }
}
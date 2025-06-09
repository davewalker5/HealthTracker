using HealthTracker.Data;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Exceptions;
using HealthTracker.DataExchange.Extensions;
using HealthTracker.DataExchange.Import;
using HealthTracker.DataExchange.Interfaces;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Food;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.Exercise
{
    [TestClass]
    public class FoodItemImportTest
    {
        private FoodCategory _category;
        private ExportableFoodItem _exportable;
        private string _filePath;

        private IHealthTrackerFactory _factory;
        private IFoodItemImporter _importer;

        [TestInitialize]
        public async Task Initialise()
        {
            var item = DataGenerator.RandomFoodItem();

            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            await context.FoodCategories.AddAsync(item.FoodCategory);
            await context.NutritionalValues.AddAsync(item.NutritionalValue);
            await context.SaveChangesAsync();

            item.FoodCategoryId = item.FoodCategory.Id;
            item.NutritionalValueId = item.NutritionalValue.Id;

            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            _importer = new FoodItemImporter(_factory, ExportableFoodItem.CsvRecordPattern);

            _category = item.FoodCategory;
            _exportable = item.ToExportable();
        }

        [TestCleanup]
        public void CleanUp()
        {
            if (!string.IsNullOrEmpty(_filePath) && File.Exists(_filePath))
            {
                // File.Delete(_filePath);
                Console.WriteLine(_filePath);
            }
        }

        [TestMethod]
        public void ConvertSingleObjectFromExportable()
        {
            var item = _exportable.FromExportable([_category]);
            Assert.AreEqual(_exportable.Name, item.Name);
            Assert.AreEqual(_exportable.Portion, item.Portion);
            Assert.AreEqual(_exportable.FoodCategory, item.FoodCategory.Name);
            Assert.AreEqual(_exportable.Calories, item.NutritionalValue.Calories);
            Assert.AreEqual(_exportable.Fat, item.NutritionalValue.Fat);
            Assert.AreEqual(_exportable.SaturatedFat, item.NutritionalValue.SaturatedFat);
            Assert.AreEqual(_exportable.Protein, item.NutritionalValue.Protein);
            Assert.AreEqual(_exportable.Carbohydrates, item.NutritionalValue.Carbohydrates);
            Assert.AreEqual(_exportable.Sugar, item.NutritionalValue.Sugar);
            Assert.AreEqual(_exportable.Fibre, item.NutritionalValue.Fibre);
        }

        [TestMethod]
        public void ConvertCollectionFromExportable()
        {
            List<ExportableFoodItem> exportable = [_exportable];
            var items = exportable.FromExportable([_category]);
            Assert.AreEqual(_exportable.Name, items.First().Name);
            Assert.AreEqual(_exportable.Portion, items.First().Portion);
            Assert.AreEqual(_exportable.FoodCategory, items.First().FoodCategory.Name);
            Assert.AreEqual(_exportable.Calories, items.First().NutritionalValue.Calories);
            Assert.AreEqual(_exportable.Fat, items.First().NutritionalValue.Fat);
            Assert.AreEqual(_exportable.SaturatedFat, items.First().NutritionalValue.SaturatedFat);
            Assert.AreEqual(_exportable.Protein, items.First().NutritionalValue.Protein);
            Assert.AreEqual(_exportable.Carbohydrates, items.First().NutritionalValue.Carbohydrates);
            Assert.AreEqual(_exportable.Sugar, items.First().NutritionalValue.Sugar);
            Assert.AreEqual(_exportable.Fibre, items.First().NutritionalValue.Fibre);
        }

        [TestMethod]
        public async Task ImportTest()
        {
            var record = $@"""{_exportable.Name}"",""{_exportable.FoodCategory}"",""{_exportable.Portion}"",""{_exportable.Calories}"",""{_exportable.Fat}"",""{_exportable.SaturatedFat}"",""{_exportable.Protein}"",""{_exportable.Carbohydrates}"",""{_exportable.Sugar}"",""{_exportable.Fibre}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var items = await _factory.FoodItems.ListAsync(x => true, 1, int.MaxValue);
            Assert.AreEqual(1, items.Count);
            Assert.AreEqual(_exportable.Name, items.First().Name);
            Assert.AreEqual(_exportable.Portion, items.First().Portion);
            Assert.AreEqual(_exportable.FoodCategory, items.First().FoodCategory.Name);
            Assert.AreEqual(_exportable.Calories, items.First().NutritionalValue.Calories);
            Assert.AreEqual(_exportable.Fat, items.First().NutritionalValue.Fat);
            Assert.AreEqual(_exportable.SaturatedFat, items.First().NutritionalValue.SaturatedFat);
            Assert.AreEqual(_exportable.Protein, items.First().NutritionalValue.Protein);
            Assert.AreEqual(_exportable.Carbohydrates, items.First().NutritionalValue.Carbohydrates);
            Assert.AreEqual(_exportable.Sugar, items.First().NutritionalValue.Sugar);
            Assert.AreEqual(_exportable.Fibre, items.First().NutritionalValue.Fibre);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRecordFormatException))]
        public async Task InvalidRecordFormatTest()
        {
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", "Invalid record format"]);

            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var factory = new HealthTrackerFactory(context, null, null);
            var importer = new FoodItemImporter(factory, ExportableFoodItem.CsvRecordPattern);
            await importer.ImportAsync(_filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidNameTest()
        {
            var record = $@""""",""{_exportable.FoodCategory}"",""{_exportable.Portion}"",""{_exportable.Calories}"",""{_exportable.Fat}"",""{_exportable.SaturatedFat}"",""{_exportable.Protein}"",""{_exportable.Carbohydrates}"",""{_exportable.Sugar}"",""{_exportable.Fibre}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidFoodCategoryTest()
        {
            var record = $@"""{_exportable.Name}"","""",""{_exportable.Portion}"",""{_exportable.Calories}"",""{_exportable.Fat}"",""{_exportable.SaturatedFat}"",""{_exportable.Protein}"",""{_exportable.Carbohydrates}"",""{_exportable.Sugar}"",""{_exportable.Fibre}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidPortionTest()
        {
            var record = $@"""{_exportable.Name}"",""{_exportable.FoodCategory}"",""0"",""{_exportable.Calories}"",""{_exportable.Fat}"",""{_exportable.SaturatedFat}"",""{_exportable.Protein}"",""{_exportable.Carbohydrates}"",""{_exportable.Sugar}"",""{_exportable.Fibre}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);
        }
    }
}
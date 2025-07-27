using HealthTracker.Data;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Exceptions;
using HealthTracker.DataExchange.Extensions;
using HealthTracker.DataExchange.Import;
using HealthTracker.Entities.Food;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.MealFoodItems
{
    [TestClass]
    public class MealFoodItemImportTest
    {
        private const decimal Quantity = 1M;

        private IHealthTrackerFactory _factory;
        private Meal _meal;
        private FoodItem _foodItem;
        private ExportableMealFoodItem _exportable;
        private string _filePath;

        [TestInitialize]
        public async Task Initialise()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);

            var foodCategoryId = (await _factory.FoodCategories.AddAsync(DataGenerator.RandomTitleCasePhrase(3, 5, 15))).Id;
            var nutritionalValue = await _factory.NutritionalValues.AddAsync(DataGenerator.RandomNutritionalValue());
            _foodItem = await _factory.FoodItems.AddAsync(
                DataGenerator.RandomTitleCasePhrase(3, 5, 15), DataGenerator.RandomDecimal(1, 100), foodCategoryId, nutritionalValue.Id);
            Console.WriteLine($"Added food item: {_foodItem}");

            var foodSourceId = (await _factory.FoodSources.AddAsync(DataGenerator.RandomTitleCasePhrase(3, 5, 15))).Id;
            _meal = await _factory.Meals.AddAsync(DataGenerator.RandomTitleCasePhrase(3, 5, 15), 1, foodSourceId, null, null);
            Console.WriteLine($"Added meal: {_meal}");

            var relationship = await _factory.MealFoodItems.AddAsync(_meal.Id, _foodItem.Id, Quantity);
            _exportable = relationship.ToExportable([_meal], [_foodItem]);
            await _factory.MealFoodItems.DeleteAsync(relationship.Id);
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
            var relationship = _exportable.FromExportable([_meal], [_foodItem]);
            Assert.AreEqual(_meal.Id, relationship.MealId);
            Assert.AreEqual(_foodItem.Id, relationship.FoodItemId);
            Assert.AreEqual(Quantity, relationship.Quantity);
            Assert.AreEqual(_exportable.Calories, relationship.NutritionalValue.Calories);
            Assert.AreEqual(_exportable.Fat, relationship.NutritionalValue.Fat);
            Assert.AreEqual(_exportable.SaturatedFat, relationship.NutritionalValue.SaturatedFat);
            Assert.AreEqual(_exportable.Protein, relationship.NutritionalValue.Protein);
            Assert.AreEqual(_exportable.Carbohydrates, relationship.NutritionalValue.Carbohydrates);
            Assert.AreEqual(_exportable.Sugar, relationship.NutritionalValue.Sugar);
            Assert.AreEqual(_exportable.Fibre, relationship.NutritionalValue.Fibre);
        }

        [TestMethod]
        public void ConvertCollectionFromExportable()
        {
            List<ExportableMealFoodItem> exportable = [_exportable];
            var relationships = exportable.FromExportable([_meal], [_foodItem]);
            Assert.AreEqual(_meal.Id, relationships.First().MealId);
            Assert.AreEqual(_foodItem.Id, relationships.First().FoodItemId);
            Assert.AreEqual(Quantity, relationships.First().Quantity);
            Assert.AreEqual(_exportable.Calories, relationships.First().NutritionalValue.Calories);
            Assert.AreEqual(_exportable.Fat, relationships.First().NutritionalValue.Fat);
            Assert.AreEqual(_exportable.SaturatedFat, relationships.First().NutritionalValue.SaturatedFat);
            Assert.AreEqual(_exportable.Protein, relationships.First().NutritionalValue.Protein);
            Assert.AreEqual(_exportable.Carbohydrates, relationships.First().NutritionalValue.Carbohydrates);
            Assert.AreEqual(_exportable.Sugar, relationships.First().NutritionalValue.Sugar);
            Assert.AreEqual(_exportable.Fibre, relationships.First().NutritionalValue.Fibre);
        }

        [TestMethod]
        public async Task ImportMeasurementsTest()
        {
            var importer = new MealFoodItemImporter(_factory, ExportableMealFoodItem.CsvRecordPattern);

            var record = $@"""{_meal.Name}"",""{_foodItem.Name}"",""{Quantity}"",""{_exportable.Calories}"",""{_exportable.Fat}"",""{_exportable.SaturatedFat}"",""{_exportable.Protein}"",""{_exportable.Carbohydrates}"",""{_exportable.Sugar}"",""{_exportable.Fibre}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await importer.ImportAsync(_filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var relationships = await _factory.MealFoodItems.ListAsync(x => x.MealId == _meal.Id);
            Assert.AreEqual(1, relationships.Count);
            Assert.AreEqual(_meal.Id, relationships.First().MealId);
            Assert.AreEqual(_foodItem.Id, relationships.First().FoodItemId);
            Assert.AreEqual(Quantity, relationships.First().Quantity);
            Assert.AreEqual(_exportable.Calories, relationships.First().NutritionalValue.Calories);
            Assert.AreEqual(_exportable.Fat, relationships.First().NutritionalValue.Fat);
            Assert.AreEqual(_exportable.SaturatedFat, relationships.First().NutritionalValue.SaturatedFat);
            Assert.AreEqual(_exportable.Protein, relationships.First().NutritionalValue.Protein);
            Assert.AreEqual(_exportable.Carbohydrates, relationships.First().NutritionalValue.Carbohydrates);
            Assert.AreEqual(_exportable.Sugar, relationships.First().NutritionalValue.Sugar);
            Assert.AreEqual(_exportable.Fibre, relationships.First().NutritionalValue.Fibre);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRecordFormatException))]
        public async Task InvalidRecordFormatTest()
        {
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", "Invalid record format"]);
            var importer = new MealFoodItemImporter(_factory, ExportableMealFoodItem.CsvRecordPattern);
            await importer.ImportAsync(_filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidMealTest()
        {
            var record = $@"""Not a valid meal"",""{_foodItem.Name}"",""{Quantity}"",""{_exportable.Calories}"",""{_exportable.Fat}"",""{_exportable.SaturatedFat}"",""{_exportable.Protein}"",""{_exportable.Carbohydrates}"",""{_exportable.Sugar}"",""{_exportable.Fibre}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);
            var importer = new MealFoodItemImporter(_factory, ExportableMealFoodItem.CsvRecordPattern);
            await importer.ImportAsync(_filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidFoodItemTest()
        {
            var record = $@"""{_meal.Name}"",""Not a valid food item"",""{Quantity}"",""{_exportable.Calories}"",""{_exportable.Fat}"",""{_exportable.SaturatedFat}"",""{_exportable.Protein}"",""{_exportable.Carbohydrates}"",""{_exportable.Sugar}"",""{_exportable.Fibre}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);
            var importer = new MealFoodItemImporter(_factory, ExportableMealFoodItem.CsvRecordPattern);
            await importer.ImportAsync(_filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFieldValueException))]
        public async Task InvalidQuantityTest()
        {
            var record = $@"""{_meal.Name}"",""{_foodItem.Name}"",""0"",""{_exportable.Calories}"",""{_exportable.Fat}"",""{_exportable.SaturatedFat}"",""{_exportable.Protein}"",""{_exportable.Carbohydrates}"",""{_exportable.Sugar}"",""{_exportable.Fibre}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);
            var importer = new MealFoodItemImporter(_factory, ExportableMealFoodItem.CsvRecordPattern);
            await importer.ImportAsync(_filePath);
        }
    }
}
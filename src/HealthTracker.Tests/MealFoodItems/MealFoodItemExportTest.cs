using HealthTracker.Data;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Export;
using HealthTracker.DataExchange.Extensions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Food;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.MealFoodItems
{
    [TestClass]
    public class MealFoodItemExportTest
    {
        private const decimal Quantity = 1M;

        private IHealthTrackerFactory _factory;
        private Meal _meal;
        private FoodItem _foodItem;
        private MealFoodItem _relationship;
        private string _filePath;

        [TestInitialize]
        public async Task Initialize()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);

            var foodCategoryId = (await _factory.FoodCategories.AddAsync(DataGenerator.RandomTitleCasePhrase(3, 5, 15))).Id;
            var nutritionalValueId = (await _factory.NutritionalValues.AddAsync(DataGenerator.RandomNutritionalValue())).Id;
            _foodItem = await _factory.FoodItems.AddAsync(
                DataGenerator.RandomTitleCasePhrase(3, 5, 15), DataGenerator.RandomDecimal(1, 100), foodCategoryId, nutritionalValueId);
            Console.WriteLine($"Added food item: {_foodItem}");

            var foodSourceId = (await _factory.FoodSources.AddAsync(DataGenerator.RandomTitleCasePhrase(3, 5, 15))).Id;
            _meal = await _factory.Meals.AddAsync(DataGenerator.RandomTitleCasePhrase(3, 5, 15), 1, foodSourceId, null, null);
            Console.WriteLine($"Added meal: {_meal}");

            _relationship = await _factory.MealFoodItems.AddAsync(_meal.Id, _foodItem.Id, Quantity);
            Console.WriteLine($"Added relationship: {_relationship}");
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
            var exportable = _relationship.ToExportable([_meal], [_foodItem]);
            Assert.AreEqual(_meal.Name, exportable.Meal);
            Assert.AreEqual(_foodItem.Name, exportable.FoodItem);
            Assert.AreEqual(Quantity, exportable.Quantity);
            Assert.AreEqual(_relationship.NutritionalValue.Calories, exportable.Calories);
            Assert.AreEqual(_relationship.NutritionalValue.Fat, exportable.Fat);
            Assert.AreEqual(_relationship.NutritionalValue.SaturatedFat, exportable.SaturatedFat);
            Assert.AreEqual(_relationship.NutritionalValue.Protein, exportable.Protein);
            Assert.AreEqual(_relationship.NutritionalValue.Carbohydrates, exportable.Carbohydrates);
            Assert.AreEqual(_relationship.NutritionalValue.Sugar, exportable.Sugar);
            Assert.AreEqual(_relationship.NutritionalValue.Fibre, exportable.Fibre);
        }

        [TestMethod]
        public void ConvertCollectionToExportable()
        {
            List<MealFoodItem> relationships = [_relationship];
            var exportable = relationships.ToExportable([_meal], [_foodItem]);
            Assert.AreEqual(_meal.Name, exportable.First().Meal);
            Assert.AreEqual(_foodItem.Name, exportable.First().FoodItem);
            Assert.AreEqual(Quantity, exportable.First().Quantity);
            Assert.AreEqual(_relationship.NutritionalValue.Calories, exportable.First().Calories);
            Assert.AreEqual(_relationship.NutritionalValue.Fat, exportable.First().Fat);
            Assert.AreEqual(_relationship.NutritionalValue.SaturatedFat, exportable.First().SaturatedFat);
            Assert.AreEqual(_relationship.NutritionalValue.Protein, exportable.First().Protein);
            Assert.AreEqual(_relationship.NutritionalValue.Carbohydrates, exportable.First().Carbohydrates);
            Assert.AreEqual(_relationship.NutritionalValue.Sugar, exportable.First().Sugar);
            Assert.AreEqual(_relationship.NutritionalValue.Fibre, exportable.First().Fibre);
        }

        [TestMethod]
        public void FromCsvRecordTest()
        {
            var record = $@"""{_meal.Name}"",""{_foodItem.Name}"",""{Quantity}"",""{_relationship.NutritionalValue.Calories}"",""{_relationship.NutritionalValue.Fat}"",""{_relationship.NutritionalValue.SaturatedFat}"",""{_relationship.NutritionalValue.Protein}"",""{_relationship.NutritionalValue.Carbohydrates}"",""{_relationship.NutritionalValue.Sugar}"",""{_relationship.NutritionalValue.Fibre}""";
            var exportable = ExportableMealFoodItem.FromCsv(record);
            Assert.AreEqual(_meal.Name, exportable.Meal);
            Assert.AreEqual(_foodItem.Name, exportable.FoodItem);
            Assert.AreEqual(Quantity, exportable.Quantity);
            Assert.AreEqual(_relationship.NutritionalValue.Calories, exportable.Calories);
            Assert.AreEqual(_relationship.NutritionalValue.Fat, exportable.Fat);
            Assert.AreEqual(_relationship.NutritionalValue.SaturatedFat, exportable.SaturatedFat);
            Assert.AreEqual(_relationship.NutritionalValue.Protein, exportable.Protein);
            Assert.AreEqual(_relationship.NutritionalValue.Carbohydrates, exportable.Carbohydrates);
            Assert.AreEqual(_relationship.NutritionalValue.Sugar, exportable.Sugar);
            Assert.AreEqual(_relationship.NutritionalValue.Fibre, exportable.Fibre);
        }

        [TestMethod]
        public async Task ExportAllMeasurementsTest()
        {
            var exporter = new MealFoodItemExporter(_factory);

            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            List<MealFoodItem> relationships = [_relationship];
            await exporter.ExportAsync(relationships, _filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var records = File.ReadAllLines(_filePath);
            Assert.AreEqual(2, records.Length);

            var exportable = ExportableMealFoodItem.FromCsv(records[1]);
            Assert.AreEqual(_meal.Name, exportable.Meal);
            Assert.AreEqual(_foodItem.Name, exportable.FoodItem);
            Assert.AreEqual(Quantity, exportable.Quantity);
            Assert.AreEqual(_relationship.NutritionalValue.Calories, exportable.Calories);
            Assert.AreEqual(_relationship.NutritionalValue.Fat, exportable.Fat);
            Assert.AreEqual(_relationship.NutritionalValue.SaturatedFat, exportable.SaturatedFat);
            Assert.AreEqual(_relationship.NutritionalValue.Protein, exportable.Protein);
            Assert.AreEqual(_relationship.NutritionalValue.Carbohydrates, exportable.Carbohydrates);
            Assert.AreEqual(_relationship.NutritionalValue.Sugar, exportable.Sugar);
            Assert.AreEqual(_relationship.NutritionalValue.Fibre, exportable.Fibre);
        }
    }
}
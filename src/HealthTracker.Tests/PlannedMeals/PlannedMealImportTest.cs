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

namespace HealthTracker.Tests.PlannedMeals
{
    [TestClass]
    public class PlannedMealImportTest
    {
        private Meal _meal;
        private ExportablePlannedMeal _exportable;
        private string _filePath;

        private IHealthTrackerFactory _factory;
        private IPlannedMealImporter _importer;

        [TestInitialize]
        public async Task Initialise()
        {
            var plannedMeal = DataGenerator.RandomPlannedMeal();

            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            await context.FoodSources.AddAsync(plannedMeal.Meal.FoodSource);
            await context.NutritionalValues.AddAsync(plannedMeal.Meal.NutritionalValue);
            await context.SaveChangesAsync();

            plannedMeal.Meal.FoodSourceId = plannedMeal.Meal.FoodSource.Id;
            plannedMeal.Meal.NutritionalValueId = plannedMeal.Meal.NutritionalValue.Id;
            await context.Meals.AddAsync(plannedMeal.Meal);
            await context.SaveChangesAsync();

            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            _importer = new PlannedMealImporter(_factory, ExportablePlannedMeal.CsvRecordPattern);

            _meal = plannedMeal.Meal;
            _exportable = plannedMeal.ToExportable();
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
            var plannedMeal = _exportable.FromExportable([_meal]);
            Assert.AreEqual(_exportable.MealType, plannedMeal.MealType.ToString());
            Assert.AreEqual(_exportable.Date, plannedMeal.Date);
            Assert.AreEqual(_meal.Id, plannedMeal.MealId);
        }

        [TestMethod]
        public void ConvertCollectionFromExportable()
        {
            List<ExportablePlannedMeal> exportable = [_exportable];
            var plannedMeals = exportable.FromExportable([_meal]);
            Assert.AreEqual(_exportable.MealType, plannedMeals.First().MealType.ToString());
            Assert.AreEqual(_exportable.Date, plannedMeals.First().Date);
            Assert.AreEqual(_meal.Id, plannedMeals.First().MealId);
        }

        [TestMethod]
        public async Task ImportTest()
        {
            var record = $@"""{_exportable.MealType}"",""{_exportable.Date:dd/MM/yyyy}"",""{_exportable.Meal}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var plannedMeals = await _factory.PlannedMeals.ListAsync(x => true, 1, int.MaxValue);
            Assert.AreEqual(1, plannedMeals.Count);
            Assert.AreEqual(_exportable.MealType, plannedMeals.First().MealType.ToString());
            Assert.AreEqual(_exportable.Date, plannedMeals.First().Date);
            Assert.AreEqual(_meal.Id, plannedMeals.First().MealId);
        }

        // [TestMethod]
        // [ExpectedException(typeof(InvalidRecordFormatException))]
        // public async Task InvalidRecordFormatTest()
        // {
        //     _filePath = DataGenerator.TemporaryCsvFilePath();
        //     File.WriteAllLines(_filePath, ["", "Invalid record format"]);

        //     var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
        //     var factory = new HealthTrackerFactory(context, null, null);
        //     var importer = new PlannedMealImporter(factory, ExportablePlannedMeal.CsvRecordPattern);
        //     await importer.ImportAsync(_filePath);
        // }

        // [TestMethod]
        // [ExpectedException(typeof(InvalidFieldValueException))]
        // public async Task InvalidNameTest()
        // {
        //     var record = $@""""",""{_exportable.FoodSource}"",""{_exportable.Reference}"",""{_exportable.Portions}"",""{_exportable.Calories}"",""{_exportable.Fat}"",""{_exportable.SaturatedFat}"",""{_exportable.Protein}"",""{_exportable.Carbohydrates}"",""{_exportable.Sugar}"",""{_exportable.Fibre}""";
        //     _filePath = DataGenerator.TemporaryCsvFilePath();
        //     File.WriteAllLines(_filePath, ["", record]);

        //     await _importer.ImportAsync(_filePath);
        // }

        // [TestMethod]
        // [ExpectedException(typeof(InvalidFieldValueException))]
        // public async Task InvalidFoodSourceTest()
        // {
        //     var record = $@"""{_exportable.Name}"","""",""{_exportable.Reference}"",""{_exportable.Portions}"",""{_exportable.Calories}"",""{_exportable.Fat}"",""{_exportable.SaturatedFat}"",""{_exportable.Protein}"",""{_exportable.Carbohydrates}"",""{_exportable.Sugar}"",""{_exportable.Fibre}""";
        //     _filePath = DataGenerator.TemporaryCsvFilePath();
        //     File.WriteAllLines(_filePath, ["", record]);

        //     await _importer.ImportAsync(_filePath);
        // }

        // [TestMethod]
        // [ExpectedException(typeof(InvalidFieldValueException))]
        // public async Task InvalidPortionsTest()
        // {
        //     var record = $@"""{_exportable.Name}"",""{_exportable.FoodSource}"",""{_exportable.Reference}"",""0"",""{_exportable.Calories}"",""{_exportable.Fat}"",""{_exportable.SaturatedFat}"",""{_exportable.Protein}"",""{_exportable.Carbohydrates}"",""{_exportable.Sugar}"",""{_exportable.Fibre}""";
        //     _filePath = DataGenerator.TemporaryCsvFilePath();
        //     File.WriteAllLines(_filePath, ["", record]);

        //     await _importer.ImportAsync(_filePath);
        // }
    }
}
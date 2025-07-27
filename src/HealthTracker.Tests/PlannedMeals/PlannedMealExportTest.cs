using HealthTracker.Data;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Export;
using HealthTracker.DataExchange.Extensions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Food;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.PlannedMeals
{
    [TestClass]
    public class PlannedMealExportTest
    {
        private readonly PlannedMeal _plannedMeal = DataGenerator.RandomPlannedMeal();

        private string _filePath;

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
            var exportable = _plannedMeal.ToExportable();
            Assert.AreEqual(_plannedMeal.MealType.ToString(), exportable.MealType);
            Assert.AreEqual(_plannedMeal.Date, exportable.Date);
            Assert.AreEqual(_plannedMeal.Meal.Name, exportable.Meal);
        }

        [TestMethod]
        public void ConvertCollectionToExportable()
        {
            List<PlannedMeal> plannedMeals = [_plannedMeal];
            var exportable = plannedMeals.ToExportable();
            Assert.AreEqual(_plannedMeal.MealType.ToString(), exportable.First().MealType);
            Assert.AreEqual(_plannedMeal.Date, exportable.First().Date);
            Assert.AreEqual(_plannedMeal.Meal.Name, exportable.First().Meal);
        }

        [TestMethod]
        public void FromCsvRecordTest()
        {
            var record = $@"""{_plannedMeal.MealType}"",""{_plannedMeal.Date:dd/MM/yyyy}"",""{_plannedMeal.Meal.Name}""";
            var exportable = ExportablePlannedMeal.FromCsv(record);
            Assert.AreEqual(_plannedMeal.MealType.ToString(), exportable.MealType);
            Assert.AreEqual(_plannedMeal.Date, exportable.Date);
            Assert.AreEqual(_plannedMeal.Meal.Name, exportable.Meal);
        }

        [TestMethod]
        public async Task ExportTest()
        {
            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            await context.FoodSources.AddAsync(_plannedMeal.Meal.FoodSource);
            await context.NutritionalValues.AddAsync(_plannedMeal.Meal.NutritionalValue);
            await context.SaveChangesAsync();

            _plannedMeal.Meal.FoodSourceId = _plannedMeal.Meal.FoodSource.Id;
            _plannedMeal.Meal.NutritionalValueId = _plannedMeal.Meal.NutritionalValue.Id;
            await context.Meals.AddAsync(_plannedMeal.Meal);
            await context.SaveChangesAsync();

            _plannedMeal.MealId = _plannedMeal.Meal.Id;
            await context.PlannedMeals.AddAsync(_plannedMeal);
            await context.SaveChangesAsync();

            var logger = new Mock<IHealthTrackerLogger>();
            var factory = new HealthTrackerFactory(context, null, logger.Object);
            var exporter = new PlannedMealExporter(factory);

            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            List<PlannedMeal> measurements = [_plannedMeal];
            await exporter.ExportAsync(measurements, _filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var records = File.ReadAllLines(_filePath);
            Assert.AreEqual(2, records.Length);

            var exportable = ExportablePlannedMeal.FromCsv(records[1]);
            Assert.AreEqual(_plannedMeal.MealType.ToString(), exportable.MealType);
            Assert.AreEqual(_plannedMeal.Date, exportable.Date);
            Assert.AreEqual(_plannedMeal.Meal.Name, exportable.Meal);
        }
    }
}
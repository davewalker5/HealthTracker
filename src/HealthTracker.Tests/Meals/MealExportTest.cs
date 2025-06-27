using HealthTracker.Data;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Export;
using HealthTracker.DataExchange.Extensions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Food;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.Meals
{
    [TestClass]
    public class MealExportTest
    {
        private readonly Meal _meal = DataGenerator.RandomMeal();

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
            var exportable = _meal.ToExportable();
            Assert.AreEqual(_meal.FoodSource.Name, exportable.FoodSource);
            Assert.AreEqual(_meal.Name, exportable.Name);
            Assert.AreEqual(_meal.Portions, exportable.Portions);
            Assert.AreEqual(_meal.NutritionalValue.Calories, exportable.Calories);
            Assert.AreEqual(_meal.NutritionalValue.Fat, exportable.Fat);
            Assert.AreEqual(_meal.NutritionalValue.SaturatedFat, exportable.SaturatedFat);
            Assert.AreEqual(_meal.NutritionalValue.Protein, exportable.Protein);
            Assert.AreEqual(_meal.NutritionalValue.Carbohydrates, exportable.Carbohydrates);
            Assert.AreEqual(_meal.NutritionalValue.Sugar, exportable.Sugar);
            Assert.AreEqual(_meal.NutritionalValue.Fibre, exportable.Fibre);
        }

        [TestMethod]
        public void ConvertCollectionToExportable()
        {
            List<Meal> items = [_meal];
            var exportable = items.ToExportable();
            Assert.AreEqual(_meal.FoodSource.Name, exportable.First().FoodSource);
            Assert.AreEqual(_meal.Name, exportable.First().Name);
            Assert.AreEqual(_meal.Portions, exportable.First().Portions);
            Assert.AreEqual(_meal.NutritionalValue.Calories, exportable.First().Calories);
            Assert.AreEqual(_meal.NutritionalValue.Fat, exportable.First().Fat);
            Assert.AreEqual(_meal.NutritionalValue.SaturatedFat, exportable.First().SaturatedFat);
            Assert.AreEqual(_meal.NutritionalValue.Protein, exportable.First().Protein);
            Assert.AreEqual(_meal.NutritionalValue.Carbohydrates, exportable.First().Carbohydrates);
            Assert.AreEqual(_meal.NutritionalValue.Sugar, exportable.First().Sugar);
            Assert.AreEqual(_meal.NutritionalValue.Fibre, exportable.First().Fibre);
        }

        [TestMethod]
        public void FromCsvRecordTest()
        {
            var record = $@"""{_meal.Name}"",""{_meal.FoodSource.Name}"",""{_meal.Portions}"",""{_meal.NutritionalValue.Calories}"",""{_meal.NutritionalValue.Fat}"",""{_meal.NutritionalValue.SaturatedFat}"",""{_meal.NutritionalValue.Protein}"",""{_meal.NutritionalValue.Carbohydrates}"",""{_meal.NutritionalValue.Sugar}"",""{_meal.NutritionalValue.Fibre}""";
            var exportable = ExportableMeal.FromCsv(record);
            Assert.AreEqual(_meal.FoodSource.Name, exportable.FoodSource);
            Assert.AreEqual(_meal.Name, exportable.Name);
            Assert.AreEqual(_meal.Portions, exportable.Portions);
            Assert.AreEqual(_meal.NutritionalValue.Calories, exportable.Calories);
            Assert.AreEqual(_meal.NutritionalValue.Fat, exportable.Fat);
            Assert.AreEqual(_meal.NutritionalValue.SaturatedFat, exportable.SaturatedFat);
            Assert.AreEqual(_meal.NutritionalValue.Protein, exportable.Protein);
            Assert.AreEqual(_meal.NutritionalValue.Carbohydrates, exportable.Carbohydrates);
            Assert.AreEqual(_meal.NutritionalValue.Sugar, exportable.Sugar);
            Assert.AreEqual(_meal.NutritionalValue.Fibre, exportable.Fibre);
        }

        [TestMethod]
        public async Task ExportTest()
        {
            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            await context.NutritionalValues.AddAsync(_meal.NutritionalValue);
            await context.SaveChangesAsync();
            _meal.NutritionalValueId = _meal.NutritionalValue.Id;
            await context.Meals.AddAsync(_meal);
            await context.SaveChangesAsync();

            var logger = new Mock<IHealthTrackerLogger>();
            var factory = new HealthTrackerFactory(context, null, logger.Object);
            var exporter = new MealExporter(factory);

            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            List<Meal> measurements = [_meal];
            await exporter.ExportAsync(measurements, _filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var records = File.ReadAllLines(_filePath);
            Assert.AreEqual(2, records.Length);

            var exportable = ExportableMeal.FromCsv(records[1]);
            Assert.AreEqual(_meal.FoodSource.Name, exportable.FoodSource);
            Assert.AreEqual(_meal.Name, exportable.Name);
            Assert.AreEqual(_meal.Portions, exportable.Portions);
            Assert.AreEqual(_meal.NutritionalValue.Calories, exportable.Calories);
            Assert.AreEqual(_meal.NutritionalValue.Fat, exportable.Fat);
            Assert.AreEqual(_meal.NutritionalValue.SaturatedFat, exportable.SaturatedFat);
            Assert.AreEqual(_meal.NutritionalValue.Protein, exportable.Protein);
            Assert.AreEqual(_meal.NutritionalValue.Carbohydrates, exportable.Carbohydrates);
            Assert.AreEqual(_meal.NutritionalValue.Sugar, exportable.Sugar);
            Assert.AreEqual(_meal.NutritionalValue.Fibre, exportable.Fibre);
        }
    }
}
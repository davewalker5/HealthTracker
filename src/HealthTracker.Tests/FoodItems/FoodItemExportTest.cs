using HealthTracker.Data;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Export;
using HealthTracker.DataExchange.Extensions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Food;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.Exercise
{
    [TestClass]
    public class FoodItemExportTest
    {
        private readonly FoodItem _item = DataGenerator.RandomFoodItem();

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
            var exportable = _item.ToExportable();
            Assert.AreEqual(_item.FoodCategory.Name, exportable.FoodCategory);
            Assert.AreEqual(_item.Name, exportable.Name);
            Assert.AreEqual(_item.Portion, exportable.Portion);
            Assert.AreEqual(_item.NutritionalValue.Calories, exportable.Calories);
            Assert.AreEqual(_item.NutritionalValue.Fat, exportable.Fat);
            Assert.AreEqual(_item.NutritionalValue.SaturatedFat, exportable.SaturatedFat);
            Assert.AreEqual(_item.NutritionalValue.Protein, exportable.Protein);
            Assert.AreEqual(_item.NutritionalValue.Carbohydrates, exportable.Carbohydrates);
            Assert.AreEqual(_item.NutritionalValue.Sugar, exportable.Sugar);
            Assert.AreEqual(_item.NutritionalValue.Fibre, exportable.Fibre);
        }

        [TestMethod]
        public void ConvertCollectionToExportable()
        {
            List<FoodItem> items = [_item];
            var exportable = items.ToExportable();
            Assert.AreEqual(_item.FoodCategory.Name, exportable.First().FoodCategory);
            Assert.AreEqual(_item.Name, exportable.First().Name);
            Assert.AreEqual(_item.Portion, exportable.First().Portion);
            Assert.AreEqual(_item.NutritionalValue.Calories, exportable.First().Calories);
            Assert.AreEqual(_item.NutritionalValue.Fat, exportable.First().Fat);
            Assert.AreEqual(_item.NutritionalValue.SaturatedFat, exportable.First().SaturatedFat);
            Assert.AreEqual(_item.NutritionalValue.Protein, exportable.First().Protein);
            Assert.AreEqual(_item.NutritionalValue.Carbohydrates, exportable.First().Carbohydrates);
            Assert.AreEqual(_item.NutritionalValue.Sugar, exportable.First().Sugar);
            Assert.AreEqual(_item.NutritionalValue.Fibre, exportable.First().Fibre);
        }

        [TestMethod]
        public void FromCsvRecordTest()
        {
            var record = $@"""{_item.Name}"",""{_item.FoodCategory.Name}"",""{_item.Portion}"",""{_item.NutritionalValue.Calories}"",""{_item.NutritionalValue.Fat}"",""{_item.NutritionalValue.SaturatedFat}"",""{_item.NutritionalValue.Protein}"",""{_item.NutritionalValue.Carbohydrates}"",""{_item.NutritionalValue.Sugar}"",""{_item.NutritionalValue.Fibre}""";
            var exportable = ExportableFoodItem.FromCsv(record);
            Assert.AreEqual(_item.FoodCategory.Name, exportable.FoodCategory);
            Assert.AreEqual(_item.Name, exportable.Name);
            Assert.AreEqual(_item.Portion, exportable.Portion);
            Assert.AreEqual(_item.NutritionalValue.Calories, exportable.Calories);
            Assert.AreEqual(_item.NutritionalValue.Fat, exportable.Fat);
            Assert.AreEqual(_item.NutritionalValue.SaturatedFat, exportable.SaturatedFat);
            Assert.AreEqual(_item.NutritionalValue.Protein, exportable.Protein);
            Assert.AreEqual(_item.NutritionalValue.Carbohydrates, exportable.Carbohydrates);
            Assert.AreEqual(_item.NutritionalValue.Sugar, exportable.Sugar);
            Assert.AreEqual(_item.NutritionalValue.Fibre, exportable.Fibre);
        }

        [TestMethod]
        public async Task ExportTest()
        {
            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            await context.FoodCategories.AddAsync(_item.FoodCategory);
            await context.NutritionalValues.AddAsync(_item.NutritionalValue);
            await context.SaveChangesAsync();
            _item.FoodCategoryId = _item.FoodCategory.Id;
            _item.NutritionalValueId = _item.NutritionalValue.Id;
            await context.FoodItems.AddAsync(_item);
            await context.SaveChangesAsync();

            var logger = new Mock<IHealthTrackerLogger>();
            var factory = new HealthTrackerFactory(context, null, logger.Object);
            var exporter = new FoodItemExporter(factory);

            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            List<FoodItem> measurements = [_item];
            await exporter.ExportAsync(measurements, _filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var records = File.ReadAllLines(_filePath);
            Assert.AreEqual(2, records.Length);

            var exportable = ExportableFoodItem.FromCsv(records[1]);
            Assert.AreEqual(_item.FoodCategory.Name, exportable.FoodCategory);
            Assert.AreEqual(_item.Name, exportable.Name);
            Assert.AreEqual(_item.Portion, exportable.Portion);
            Assert.AreEqual(_item.NutritionalValue.Calories, exportable.Calories);
            Assert.AreEqual(_item.NutritionalValue.Fat, exportable.Fat);
            Assert.AreEqual(_item.NutritionalValue.SaturatedFat, exportable.SaturatedFat);
            Assert.AreEqual(_item.NutritionalValue.Protein, exportable.Protein);
            Assert.AreEqual(_item.NutritionalValue.Carbohydrates, exportable.Carbohydrates);
            Assert.AreEqual(_item.NutritionalValue.Sugar, exportable.Sugar);
            Assert.AreEqual(_item.NutritionalValue.Fibre, exportable.Fibre);
        }
    }
}
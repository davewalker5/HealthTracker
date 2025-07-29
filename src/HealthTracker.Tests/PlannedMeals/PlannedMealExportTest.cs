using HealthTracker.Data;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Export;
using HealthTracker.DataExchange.Extensions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Food;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;
using HealthTracker.Entities.Identity;

namespace HealthTracker.Tests.PlannedMeals
{
    [TestClass]
    public class PlannedMealExportTest
    {
        private PlannedMeal _plannedMeal;
        private Person _person;

        private string _filePath;

        [TestInitialize]
        public void Initialise()
        {
            _person = DataGenerator.RandomPerson(10, 90);
            _plannedMeal = DataGenerator.RandomPlannedMeal();
            _plannedMeal.PersonId = _person.Id;
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
            var exportable = _plannedMeal.ToExportable([_person]);
            Assert.AreEqual(_plannedMeal.MealType.ToString(), exportable.MealType);
            Assert.AreEqual(_plannedMeal.Date.Year, exportable.Date.Year);
            Assert.AreEqual(_plannedMeal.Date.Month, exportable.Date.Month);
            Assert.AreEqual(_plannedMeal.Date.Day, exportable.Date.Day);
            Assert.AreEqual(_plannedMeal.Meal.Name, exportable.Meal);
        }

        [TestMethod]
        public void ConvertCollectionToExportable()
        {
            List<PlannedMeal> plannedMeals = [_plannedMeal];
            var exportable = plannedMeals.ToExportable([_person]);
            Assert.AreEqual(_plannedMeal.PersonId, exportable.First().PersonId);
            Assert.AreEqual(_plannedMeal.Date.Year, exportable.First().Date.Year);
            Assert.AreEqual(_plannedMeal.Date.Month, exportable.First().Date.Month);
            Assert.AreEqual(_plannedMeal.Date.Day, exportable.First().Date.Day);
            Assert.AreEqual(_plannedMeal.MealType.ToString(), exportable.First().MealType);
            Assert.AreEqual(_plannedMeal.Meal.Name, exportable.First().Meal);
        }

        [TestMethod]
        public void FromCsvRecordTest()
        {
            var record = $@"""{_person.Id}"",""{_person.Name}"",""{_plannedMeal.Date:dd-MMM-yyyy HH:mm:ss}"",""{_plannedMeal.MealType}"",""{_plannedMeal.Meal.Name}"",""{_plannedMeal.Meal.FoodSource.Name}"",""{_plannedMeal.Meal.Reference}""";
            var exportable = ExportablePlannedMeal.FromCsv(record);
            Assert.AreEqual(_plannedMeal.PersonId, exportable.PersonId);
            Assert.AreEqual(_plannedMeal.Date.Year, exportable.Date.Year);
            Assert.AreEqual(_plannedMeal.Date.Month, exportable.Date.Month);
            Assert.AreEqual(_plannedMeal.Date.Day, exportable.Date.Day);
            Assert.AreEqual(_plannedMeal.MealType.ToString(), exportable.MealType);
            Assert.AreEqual(_plannedMeal.Meal.Name, exportable.Meal);
        }

        [TestMethod]
        public async Task ExportTest()
        {
            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            await context.People.AddAsync(_person);
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
            Assert.AreEqual(_plannedMeal.PersonId, exportable.PersonId);
            Assert.AreEqual(_plannedMeal.Date.Year, exportable.Date.Year);
            Assert.AreEqual(_plannedMeal.Date.Month, exportable.Date.Month);
            Assert.AreEqual(_plannedMeal.Date.Day, exportable.Date.Day);
            Assert.AreEqual(_plannedMeal.MealType.ToString(), exportable.MealType);
            Assert.AreEqual(_plannedMeal.Meal.Name, exportable.Meal);
        }
    }
}
using HealthTracker.Data;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Exceptions;
using HealthTracker.DataExchange.Extensions;
using HealthTracker.DataExchange.Import;
using HealthTracker.DataExchange.Interfaces;
using HealthTracker.Entities.Food;
using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.PlannedMeals
{
    [TestClass]
    public class PlannedMealImportTest
    {
        private Person _person = DataGenerator.RandomPerson(10, 90);
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
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);

            await context.People.AddAsync(_person);
            await context.FoodSources.AddAsync(plannedMeal.Meal.FoodSource);
            await context.NutritionalValues.AddAsync(plannedMeal.Meal.NutritionalValue);
            await context.SaveChangesAsync();

            plannedMeal.PersonId = _person.Id;
            plannedMeal.Meal.FoodSourceId = plannedMeal.Meal.FoodSource.Id;
            plannedMeal.Meal.NutritionalValueId = plannedMeal.Meal.NutritionalValue.Id;
            await context.Meals.AddAsync(plannedMeal.Meal);
            await context.SaveChangesAsync();

            _importer = new PlannedMealImporter(_factory, ExportablePlannedMeal.CsvRecordPattern);

            _meal = plannedMeal.Meal;
            _exportable = plannedMeal.ToExportable([_person]);
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
            Assert.AreEqual(_exportable.PersonId, plannedMeal.PersonId);
            Assert.AreEqual(_exportable.Date, plannedMeal.Date);
            Assert.AreEqual(_exportable.MealType, plannedMeal.MealType.ToString());
            Assert.AreEqual(_meal.Id, plannedMeal.MealId);
        }

        [TestMethod]
        public void ConvertCollectionFromExportable()
        {
            List<ExportablePlannedMeal> exportable = [_exportable];
            var plannedMeals = exportable.FromExportable([_meal]);
            Assert.AreEqual(_exportable.PersonId, plannedMeals.First().PersonId);
            Assert.AreEqual(_exportable.Date, plannedMeals.First().Date);
            Assert.AreEqual(_exportable.MealType, plannedMeals.First().MealType.ToString());
            Assert.AreEqual(_meal.Id, plannedMeals.First().MealId);
        }

        [TestMethod]
        public async Task ImportTest()
        {
            var record = $@"""{_person.Id}"",""{_person.Name}"",""{_exportable.Date:dd-MMM-yyyy HH:mm:ss}"",""{_exportable.MealType}"",""{_exportable.Meal}"",""{_exportable.Source}"",""{_exportable.Reference}""";
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", record]);

            await _importer.ImportAsync(_filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsTrue(info.Length > 0);

            var plannedMeals = await _factory.PlannedMeals.ListAsync(x => true, 1, int.MaxValue);
            Assert.AreEqual(1, plannedMeals.Count);
            Assert.AreEqual(_exportable.MealType, plannedMeals.First().MealType.ToString());
            Assert.AreEqual(_exportable.Date.Year, plannedMeals.First().Date.Year);
            Assert.AreEqual(_exportable.Date.Month, plannedMeals.First().Date.Month);
            Assert.AreEqual(_exportable.Date.Day, plannedMeals.First().Date.Day);
            Assert.AreEqual(_meal.Id, plannedMeals.First().MealId);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRecordFormatException))]
        public async Task InvalidRecordFormatTest()
        {
            _filePath = DataGenerator.TemporaryCsvFilePath();
            File.WriteAllLines(_filePath, ["", "Invalid record format"]);

            var context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var factory = new HealthTrackerFactory(context, null, null);
            var importer = new PlannedMealImporter(factory, ExportablePlannedMeal.CsvRecordPattern);
            await importer.ImportAsync(_filePath);
        }
    }
}
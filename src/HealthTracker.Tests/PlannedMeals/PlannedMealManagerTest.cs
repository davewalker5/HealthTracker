using HealthTracker.Data;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Enumerations.Enumerations;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.PlannedMeals
{
    [TestClass]
    public class PlannedMealManagerTest
    {
        private readonly string Name = DataGenerator.RandomTitleCasePhrase(5, 5, 20);
        private readonly int Portions = DataGenerator.RandomInt(1, 10);
        private readonly string Reference = DataGenerator.RandomTitleCasePhrase(2, 5, 10);
        private readonly DateTime Date = DataGenerator.RandomDateInYear(2025);
        private readonly DateTime UpdatedDate = DataGenerator.RandomDateInYear(2025);

        private IHealthTrackerFactory _factory;
        private int _mealId;
        private int _plannedMealId;

        [TestInitialize]
        public async Task TestInitialize()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);
            var source = await _factory.FoodSources.AddAsync(DataGenerator.RandomTitleCasePhrase(3, 5, 10));
            var meal = await _factory.Meals.AddAsync(Name, Portions, source.Id, Reference, null);
            _mealId = meal.Id;
            _plannedMealId = (await _factory.PlannedMeals.AddAsync(MealType.Dinner, Date, meal.Id)).Id;
        }

        [TestMethod]
        public async Task AddAndListTest()
        {
            var plannedMeals = await _factory.PlannedMeals.ListAsync(x => true, 1, int.MaxValue);
            Assert.AreEqual(1, plannedMeals.Count);
            Assert.AreEqual(MealType.Dinner, plannedMeals.First().MealType);
            Assert.AreEqual(Date, plannedMeals.First().Date);
            Assert.AreEqual(_mealId, plannedMeals.First().MealId);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            await _factory.PlannedMeals.UpdateAsync(_plannedMealId, MealType.Lunch, UpdatedDate, _mealId);
            var plannedMeals = await _factory.PlannedMeals.ListAsync(x => true, 1, int.MaxValue);
            Assert.AreEqual(1, plannedMeals.Count);
            Assert.AreEqual(MealType.Lunch, plannedMeals.First().MealType);
            Assert.AreEqual(UpdatedDate, plannedMeals.First().Date);
            Assert.AreEqual(_mealId, plannedMeals.First().MealId);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            await _factory.PlannedMeals.DeleteAsync(_plannedMealId);
            var plannedMeals = await _factory.PlannedMeals.ListAsync(a => true, 1, int.MaxValue);
            Assert.AreEqual(0, plannedMeals.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(PlannedMealExistsException))]
        public async Task CannotCreateDuplicateTest()
            => await _factory.PlannedMeals.AddAsync(MealType.Dinner, Date, _mealId);

        [TestMethod]
        [ExpectedException(typeof(MealNotFoundException))]
        public async Task CannotAddPlannedMealForMissingMeal()
            => await _factory.PlannedMeals.AddAsync(MealType.Lunch, UpdatedDate, 10 * _mealId);
    }
}

using HealthTracker.Data;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Food;
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
        private Meal _meal;
        private int _personId;
        private PlannedMeal _plannedMeal;

        [TestInitialize]
        public async Task TestInitialize()
        {
            HealthTrackerDbContext context = HealthTrackerDbContextFactory.CreateInMemoryDbContext();
            var logger = new Mock<IHealthTrackerLogger>();
            _factory = new HealthTrackerFactory(context, null, logger.Object);

            // Set up a food item
            var foodCategory = await _factory.FoodCategories.AddAsync(DataGenerator.RandomTitleCasePhrase(3, 5, 10));
            var nutritionalValue = await _factory.NutritionalValues.AddAsync(DataGenerator.RandomNutritionalValue(0));
            var foodItem = await _factory.FoodItems.AddAsync(Name, DataGenerator.RandomDecimal(100, 500), foodCategory.Id, nutritionalValue.Id);

            // Set up a meal, with the food item as an ingredient
            var source = await _factory.FoodSources.AddAsync(DataGenerator.RandomTitleCasePhrase(3, 5, 10));
            _meal = await _factory.Meals.AddAsync(Name, Portions, source.Id, Reference, null);
            _ = await _factory.MealFoodItems.AddAsync(_meal.Id, foodItem.Id, DataGenerator.RandomDecimal(100, 500));

            // Create a person to own planned meals
            var person = DataGenerator.RandomPerson(10, 90);
            _personId = (await _factory.People.AddAsync(person.FirstNames, person.Surname, person.DateOfBirth, person.Height, person.Gender)).Id;
            await context.SaveChangesAsync();

            // Set up a planned meal
            _plannedMeal = await _factory.PlannedMeals.AddAsync(_personId, MealType.Dinner, Date, _meal.Id);
        }

        [TestMethod]
        public async Task AddAndListTest()
        {
            var plannedMeals = await _factory.PlannedMeals.ListAsync(x => true, 1, int.MaxValue);
            Assert.AreEqual(1, plannedMeals.Count);
            Assert.AreEqual(_personId, plannedMeals.First().PersonId);
            Assert.AreEqual(Date, plannedMeals.First().Date);
            Assert.AreEqual(MealType.Dinner, plannedMeals.First().MealType);
            Assert.AreEqual(_meal.Id, plannedMeals.First().MealId);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            await _factory.PlannedMeals.UpdateAsync(_plannedMeal.Id, _personId, MealType.Lunch, UpdatedDate, _meal.Id);
            var plannedMeals = await _factory.PlannedMeals.ListAsync(x => true, 1, int.MaxValue);
            Assert.AreEqual(1, plannedMeals.Count);
            Assert.AreEqual(MealType.Lunch, plannedMeals.First().MealType);
            Assert.AreEqual(UpdatedDate, plannedMeals.First().Date);
            Assert.AreEqual(_meal.Id, plannedMeals.First().MealId);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            await _factory.PlannedMeals.DeleteAsync(_plannedMeal.Id);
            var plannedMeals = await _factory.PlannedMeals.ListAsync(a => true, 1, int.MaxValue);
            Assert.AreEqual(0, plannedMeals.Count);
        }

        [TestMethod]
        public async Task PurgeTest()
        {
            var cutoff = _plannedMeal.Date.AddDays(1);
            await _factory.PlannedMeals.PurgeAsync(_personId, cutoff);
            var plannedMeals = await _factory.PlannedMeals.ListAsync(a => true, 1, int.MaxValue);
            Assert.AreEqual(0, plannedMeals.Count);
        }

        [TestMethod]
        public async Task GetShoppingListTest()
        {
            var from = _plannedMeal.Date.AddDays(-1);
            var to = _plannedMeal.Date.AddDays(1);
            var shoppingList = await _factory.PlannedMeals.GetShoppingList(_personId, from, to);

            Assert.AreEqual(1, shoppingList.Count);
            Assert.AreEqual(_meal.MealFoodItems.First().Id, shoppingList.First().FoodItemId);
            Assert.AreEqual(_meal.MealFoodItems.First().Quantity, shoppingList.First().Portion);
            Assert.AreEqual(1, shoppingList.First().Quantity);
            Assert.AreEqual(_meal.MealFoodItems.First().FoodItem.Name, shoppingList.First().Item);
        }

        [TestMethod]
        [ExpectedException(typeof(PlannedMealExistsException))]
        public async Task CannotCreateDuplicateTest()
            => await _factory.PlannedMeals.AddAsync(_personId, MealType.Dinner, Date, _meal.Id);

        [TestMethod]
        [ExpectedException(typeof(PlannedMealNotFoundException))]
        public async Task CannotUpdateMissingPlannedMealTest()
            => await _factory.PlannedMeals.UpdateAsync(10 * _plannedMeal.Id, _personId, MealType.Lunch, UpdatedDate, _meal.Id);

        [TestMethod]
        [ExpectedException(typeof(PlannedMealNotFoundException))]
        public async Task CannotDeleteMissingPlannedMealTest()
            => await _factory.PlannedMeals.DeleteAsync(10 * _plannedMeal.Id);

        [TestMethod]
        [ExpectedException(typeof(MealNotFoundException))]
        public async Task CannotAddPlannedMealForMissingMeal()
            => await _factory.PlannedMeals.AddAsync(_personId, MealType.Lunch, UpdatedDate, 10 * _meal.Id);
    }
}

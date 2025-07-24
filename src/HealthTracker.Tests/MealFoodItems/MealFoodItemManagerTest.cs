using HealthTracker.Data;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Food;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Logic.Factory;
using HealthTracker.Tests.Mocks;
using Moq;

namespace HealthTracker.Tests.MealFoodItems
{
    [TestClass]
    public class MealFoodItemManagerTest
    {
        private const decimal Quantity = 1M;

        private IHealthTrackerFactory _factory;
        private Meal _meal;
        private FoodItem _foodItem;
        private MealFoodItem _relationship;

        [TestInitialize]
        public async Task TestInitialize()
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
            _meal = await _factory.Meals.AddAsync(DataGenerator.RandomTitleCasePhrase(3, 5, 15), 1, foodSourceId, null);
            Console.WriteLine($"Added meal: {_meal}");

            _relationship = await _factory.MealFoodItems.AddAsync(_meal.Id, _foodItem.Id, Quantity);
            Console.WriteLine($"Added relationship: {_relationship}");
        }

        private void AssertCorrectNutritionalValue(decimal? baseValue, decimal portion, decimal quantity, decimal? actual)
        {
            var expected = Math.Round(baseValue.Value * quantity / portion, 4);
            Assert.AreEqual(expected, Math.Round(actual.Value, 4));
        }

        [TestMethod]
        public async Task AddAndListTest()
        {
            var relationships = await _factory.MealFoodItems.ListAsync(x => x.MealId == _meal.Id);
            Assert.AreEqual(1, relationships.Count);
            Assert.AreEqual(_relationship.Id, relationships.First().Id);
            Assert.AreEqual(_relationship.MealId, relationships.First().MealId);
            Assert.AreEqual(_relationship.FoodItemId, relationships.First().FoodItemId);
            Assert.AreEqual(_relationship.Quantity, relationships.First().Quantity);
            Assert.IsNotNull(_relationship.NutritionalValue);
            AssertCorrectNutritionalValue(_foodItem.NutritionalValue.Calories, _foodItem.Portion, Quantity, relationships.First().NutritionalValue.Calories);
            AssertCorrectNutritionalValue(_foodItem.NutritionalValue.Fat, _foodItem.Portion, Quantity, relationships.First().NutritionalValue.Fat);
            AssertCorrectNutritionalValue(_foodItem.NutritionalValue.SaturatedFat, _foodItem.Portion, Quantity, relationships.First().NutritionalValue.SaturatedFat);
            AssertCorrectNutritionalValue(_foodItem.NutritionalValue.Protein, _foodItem.Portion, Quantity, relationships.First().NutritionalValue.Protein);
            AssertCorrectNutritionalValue(_foodItem.NutritionalValue.Carbohydrates, _foodItem.Portion, Quantity, relationships.First().NutritionalValue.Carbohydrates);
            AssertCorrectNutritionalValue(_foodItem.NutritionalValue.Sugar, _foodItem.Portion, Quantity, relationships.First().NutritionalValue.Sugar);
            AssertCorrectNutritionalValue(_foodItem.NutritionalValue.Fibre, _foodItem.Portion, Quantity, relationships.First().NutritionalValue.Fibre);

            var meal = (await _factory.Meals.ListAsync(x => x.Id == _meal.Id, 1, 1)).First();
            Assert.IsNotNull(meal.NutritionalValue);
            AssertCorrectNutritionalValue(_foodItem.NutritionalValue.Calories, _foodItem.Portion, Quantity, meal.NutritionalValue.Calories);
            AssertCorrectNutritionalValue(_foodItem.NutritionalValue.Fat, _foodItem.Portion, Quantity, meal.NutritionalValue.Fat);
            AssertCorrectNutritionalValue(_foodItem.NutritionalValue.SaturatedFat, _foodItem.Portion, Quantity, meal.NutritionalValue.SaturatedFat);
            AssertCorrectNutritionalValue(_foodItem.NutritionalValue.Protein, _foodItem.Portion, Quantity, meal.NutritionalValue.Protein);
            AssertCorrectNutritionalValue(_foodItem.NutritionalValue.Carbohydrates, _foodItem.Portion, Quantity, meal.NutritionalValue.Carbohydrates);
            AssertCorrectNutritionalValue(_foodItem.NutritionalValue.Sugar, _foodItem.Portion, Quantity, meal.NutritionalValue.Sugar);
            AssertCorrectNutritionalValue(_foodItem.NutritionalValue.Fibre, _foodItem.Portion, Quantity, meal.NutritionalValue.Fibre);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var updatedQuantity = DataGenerator.RandomDecimal(1, 5);
            await _factory.MealFoodItems.UpdateAsync(_relationship.Id, _meal.Id, _foodItem.Id, updatedQuantity);
            var relationships = await _factory.MealFoodItems.ListAsync(x => x.MealId == _meal.Id);
            Assert.AreEqual(1, relationships.Count);
            Assert.AreEqual(_relationship.Id, relationships.First().Id);
            Assert.AreEqual(_relationship.MealId, relationships.First().MealId);
            Assert.AreEqual(_relationship.FoodItemId, relationships.First().FoodItemId);
            Assert.AreEqual(_relationship.Quantity, relationships.First().Quantity);
            Assert.IsNotNull(_relationship.NutritionalValue);
            AssertCorrectNutritionalValue(_foodItem.NutritionalValue.Calories, _foodItem.Portion, updatedQuantity, relationships.First().NutritionalValue.Calories);
            AssertCorrectNutritionalValue(_foodItem.NutritionalValue.Fat, _foodItem.Portion, updatedQuantity, relationships.First().NutritionalValue.Fat);
            AssertCorrectNutritionalValue(_foodItem.NutritionalValue.SaturatedFat, _foodItem.Portion, updatedQuantity, relationships.First().NutritionalValue.SaturatedFat);
            AssertCorrectNutritionalValue(_foodItem.NutritionalValue.Protein, _foodItem.Portion, updatedQuantity, relationships.First().NutritionalValue.Protein);
            AssertCorrectNutritionalValue(_foodItem.NutritionalValue.Carbohydrates, _foodItem.Portion, updatedQuantity, relationships.First().NutritionalValue.Carbohydrates);
            AssertCorrectNutritionalValue(_foodItem.NutritionalValue.Sugar, _foodItem.Portion, updatedQuantity, relationships.First().NutritionalValue.Sugar);
            AssertCorrectNutritionalValue(_foodItem.NutritionalValue.Fibre, _foodItem.Portion, updatedQuantity, relationships.First().NutritionalValue.Fibre);

            var meal = (await _factory.Meals.ListAsync(x => x.Id == _meal.Id, 1, 1)).First();
            Assert.IsNotNull(meal.NutritionalValue);
            AssertCorrectNutritionalValue(_foodItem.NutritionalValue.Calories, _foodItem.Portion, updatedQuantity, meal.NutritionalValue.Calories);
            AssertCorrectNutritionalValue(_foodItem.NutritionalValue.Fat, _foodItem.Portion, updatedQuantity, meal.NutritionalValue.Fat);
            AssertCorrectNutritionalValue(_foodItem.NutritionalValue.SaturatedFat, _foodItem.Portion, updatedQuantity, meal.NutritionalValue.SaturatedFat);
            AssertCorrectNutritionalValue(_foodItem.NutritionalValue.Protein, _foodItem.Portion, updatedQuantity, meal.NutritionalValue.Protein);
            AssertCorrectNutritionalValue(_foodItem.NutritionalValue.Carbohydrates, _foodItem.Portion, updatedQuantity, meal.NutritionalValue.Carbohydrates);
            AssertCorrectNutritionalValue(_foodItem.NutritionalValue.Sugar, _foodItem.Portion, updatedQuantity, meal.NutritionalValue.Sugar);
            AssertCorrectNutritionalValue(_foodItem.NutritionalValue.Fibre, _foodItem.Portion, updatedQuantity, meal.NutritionalValue.Fibre);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            await _factory.MealFoodItems.DeleteAsync(_relationship.Id);
            var relationships = await _factory.MealFoodItems.ListAsync(x => x.MealId == _meal.Id);
            Assert.AreEqual(0, relationships.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(MealFoodItemExistsException))]
        public async Task CannotAddDuplicateRelationshipTest()
            => await _factory.MealFoodItems.AddAsync(_meal.Id, _foodItem.Id, DataGenerator.RandomDecimal(100, 200));

        [TestMethod]
        [ExpectedException(typeof(MealNotFoundException))]
        public async Task CannotAddRelationshipForMissingMealTest()
            => await _factory.MealFoodItems.AddAsync(100 * _meal.Id, _foodItem.Id, DataGenerator.RandomDecimal(100, 200));

        [TestMethod]
        [ExpectedException(typeof(FoodItemNotFoundException))]
        public async Task CannotAddRelationshipForMissingFoodItemTest()
            => await _factory.MealFoodItems.AddAsync(_meal.Id, 100 * _foodItem.Id, DataGenerator.RandomDecimal(100, 200));

    }
}

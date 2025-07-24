using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Food;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Logging;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HealthTracker.Logic.Database
{
    public class MealFoodItemManager : DatabaseManagerBase, IMealFoodItemManager
    {
        internal MealFoodItemManager(IHealthTrackerFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return the first meal/food item relationship matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<MealFoodItem> GetAsync(Expression<Func<MealFoodItem, bool>> predicate)
        {
            var relationships = await ListAsync(predicate);
            return relationships.FirstOrDefault();
        }

        /// <summary>
        /// Return all meal/food item relationships matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<MealFoodItem>> ListAsync(Expression<Func<MealFoodItem, bool>> predicate)
            => await Context.MealFoodItems
                            .Where(predicate)
                            .Include(x => x.NutritionalValue)
                            .Include(x => x.FoodItem)
                                .ThenInclude(x => x.FoodCategory)
                            .Include(x => x.FoodItem)
                            .ToListAsync();

        /// <summary>
        /// Add a meal/food item relationship, if it doesn't already exist
        /// </summary>
        /// <param name="mealId"></param>
        /// <param name="foodItemId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public async Task<MealFoodItem> AddAsync(int mealId, int foodItemId, decimal quantity)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Creating new meal/food item relationship : Meal ID = {mealId}, Food Item ID = {foodItemId}, Quantity = {quantity}");

            // Check the meal and food item both exist and that we're not creating a duplicate
            Factory.Meals.CheckMealExists(mealId);
            Factory.FoodItems.CheckFoodItemExists(foodItemId);
            await CheckMealFoodItemIsNotADuplicate(mealId, foodItemId, 0);

            // Add the relationship and save changes
            var relationship = new MealFoodItem
            {
                MealId = mealId,
                FoodItemId = foodItemId,
                Quantity = quantity
            };

            await Context.MealFoodItems.AddAsync(relationship);
            await Context.SaveChangesAsync();

            // Create the nutritional value for this relationship. The assumption is the units for "quantity" are the same
            // as the units for the "portion" on the food item, so the multiplier for the nutritional values is quantity
            // divided by portion
            var foodItem = (await Factory.FoodItems.ListAsync(x => x.Id == foodItemId, 1, 1)).First();
            var multiplier = quantity / foodItem.Portion;
            var calculated = Factory.NutritionalValues.CalculateNutritionalValues(foodItem.NutritionalValue, multiplier);
            var nutritionalValues = await Factory.NutritionalValues.CreateOrUpdateNutritionalValueAsync(null, calculated);

            // Update the relationship to associate the nutritional values with it
            if (nutritionalValues != null)
            {
                relationship.NutritionalValueId = nutritionalValues.Id;
                await Context.SaveChangesAsync();
            }

            // Update the associated meal's nutritional values
            await Factory.Meals.UpdateNutritionalValues(mealId);

            // Reload to load related entities
            relationship = await GetAsync(x => x.Id == relationship.Id);
            return relationship;
        }

        /// <summary>
        /// Update the properties of the specified meal/food item relationship
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mealId"></param>
        /// <param name="foodItemId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public async Task<MealFoodItem> UpdateAsync(int id, int mealId, int foodItemId, decimal quantity)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Updating meal/food item relationship : ID = {id}, Meal ID = {mealId}, Food Item ID = {foodItemId}, Quantity = {quantity}");

            var relationship = Context.MealFoodItems.FirstOrDefault(x => x.Id == id);
            if (relationship == null)
            {
                var message = $"Meal/food item relationship with ID = {id} not found";
                throw new MealFoodItemNotFoundException(message);
            }

            // Check the meal and food item both exist and that we're not creating a duplicate
            Factory.Meals.CheckMealExists(mealId);
            Factory.FoodItems.CheckFoodItemExists(foodItemId);
            await CheckMealFoodItemIsNotADuplicate(mealId, foodItemId, id);

            // Save the changes
            relationship.MealId = mealId;
            relationship.FoodItemId = foodItemId;
            relationship.Quantity = quantity;
            await Context.SaveChangesAsync();

            // Update the nutritional value for this relationship. The assumption is the units for "quantity" are the same
            // as the units for the "portion" on the food item, so the multiplier for the nutritional values is quantity
            // divided by portion
            var foodItem = (await Factory.FoodItems.ListAsync(x => x.Id == foodItemId, 1, 1)).First();
            var multiplier = quantity / foodItem.Portion;
            var calculated = Factory.NutritionalValues.CalculateNutritionalValues(foodItem.NutritionalValue, multiplier);
            var nutritionalValues = await Factory.NutritionalValues.CreateOrUpdateNutritionalValueAsync(relationship.NutritionalValueId, calculated);

            // Update the relationship to associate the nutritional values with it
            if (nutritionalValues != null)
            {
                relationship.NutritionalValueId = nutritionalValues.Id;
                await Context.SaveChangesAsync();
            }

            // Update the associated meal's nutritional values
            await Factory.Meals.UpdateNutritionalValues(mealId);

            // Reload to load related entities
            relationship = await GetAsync(x => x.Id == relationship.Id);
            return relationship;
        }

        /// <summary>
        /// Delete the meal/food item relationship with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Deleting meal/food item relationship with ID {id}");

            var relationship = await GetAsync(x => x.Id == id);
            if (relationship == null)
            {
                var message = $"Meal/food item relationship with ID = {id} not found";
                throw new MealFoodItemNotFoundException(message);
            }

            // Delete the relationship and save changes
            Factory.Context.Remove(relationship);
            await Factory.Context.SaveChangesAsync();

            // Update the associated meal's nutritional values
            await Factory.Meals.UpdateNutritionalValues(relationship.MealId);

        }

        /// <summary>
        /// Raise an exception if an attempt is made to add/update a meal/food item relationship with duplicate
        /// meal and food item IDs
        /// </summary>
        /// <param name="mealId"></param>
        /// <param name="foodItemId"></param>
        /// <param name="id"></param>
        /// <exception cref="MealFoodItemExistsException"></exception>
        private async Task CheckMealFoodItemIsNotADuplicate(int mealId, int foodItemId, int id)
        {
            var mealFoodItem = await Context.MealFoodItems.FirstOrDefaultAsync(x => (x.MealId == mealId) && (x.FoodItemId == foodItemId));
            if ((mealFoodItem != null) && (mealFoodItem.Id != id))
            {
                var message = $"Relationship between meal with ID {mealId} and food item with ID {foodItemId} already exists";
                throw new MealFoodItemExistsException(message);
            }
        }
    }
}

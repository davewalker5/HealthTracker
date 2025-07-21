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
                            .Include(x => x.FoodItem)
                            .ToListAsync();

        /// <summary>
        /// Add a meal/food item relationship, if it doesn't already exist
        /// </summary>
        /// <param name="mealId"></param>
        /// <param name="foodItemId"></param>
        /// <returns></returns>
        public async Task<MealFoodItem> AddAsync(int mealId, int foodItemId)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Creating new meal/food item relationship : Meal ID = {mealId}, Food Item ID = {foodItemId}");

            // Check the meal and food item both exist and that we're not creating a duplicate
            Factory.Meals.CheckMealExists(mealId);
            Factory.FoodItems.CheckFoodItemExists(foodItemId);
            await CheckMealFoodItemIsNotADuplicate(mealId, foodItemId, 0);

            // Add the relationship and save changes
            var relationship = new MealFoodItem
            {
                MealId = mealId,
                FoodItemId = foodItemId
            };

            await Context.MealFoodItems.AddAsync(relationship);
            await Context.SaveChangesAsync();

            // Load the related food item
            await Context.Entry(relationship).Reference(x => x.FoodItem).LoadAsync();

            return relationship;
        }

        /// <summary>
        /// Update the properties of the specified meal/food item relationship
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mealId"></param>
        /// <param name="foodItemId"></param>
        /// <returns></returns>
        public async Task<MealFoodItem> UpdateAsync(int id, int mealId, int foodItemId)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Updating meal/food item relationship : ID = {id}, Meal ID = {mealId}, Food Item ID = {foodItemId}");

            var relationship = Context.MealFoodItems.FirstOrDefault(x => x.Id == id);
            if (relationship != null)
            {
                // Check the meal and food item both exist and that we're not creating a duplicate
                Factory.Meals.CheckMealExists(mealId);
                Factory.FoodItems.CheckFoodItemExists(foodItemId);
                await CheckMealFoodItemIsNotADuplicate(mealId, foodItemId, id);

                // Save the changes
                relationship.MealId = mealId;
                relationship.FoodItemId = foodItemId;
                await Context.SaveChangesAsync();

                // Load the related food item
                await Context.Entry(relationship).Reference(x => x.FoodItem).LoadAsync();
            }

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
            if (relationship != null)
            {
                // Delete the relationship and save changes
                Factory.Context.Remove(relationship);
                await Factory.Context.SaveChangesAsync();
            }
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

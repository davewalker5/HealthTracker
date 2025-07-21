using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Logging;
using HealthTracker.Entities.Food;
using HealthTracker.Entities.Exceptions;
using HealthTracker.Logic.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HealthTracker.Logic.Database
{
    public class MealManager : MeasurementManagerBase, IMealManager
    {
        internal MealManager(IHealthTrackerFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return all meals matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Meal>> ListAsync(Expression<Func<Meal, bool>> predicate, int pageNumber, int pageSize)
            => await Context.Meals
                            .Include(x => x.FoodSource)
                            .Include(x => x.NutritionalValue)
                            .Where(predicate)
                            .OrderBy(x => x.Name)
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();

        /// <summary>
        /// Add a new meal
        /// </summary>
        /// <param name="name"></param>
        /// <param name="portion"></param>
        /// <param name="foodSourceId"></param>
        /// <param name="nutritionalValueId"></param>
        /// <returns></returns>
        public async Task<Meal> AddAsync(string name, int portions, int foodSourceId, int? nutritionalValueId)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Adding meal: Name = {name}, Portions = {portions}, Food Source ID = {foodSourceId}, Nutritional Value ID = {nutritionalValueId}");

            // Clean up the name and make sure we're not creating a duplicate and that the
            // related nutritional value exists
            var clean = StringCleaner.Clean(name);
            Factory.FoodSources.CheckFoodSourceExists(foodSourceId);
            Factory.NutritionalValues.CheckNutritionalValueExists(nutritionalValueId);
            await CheckMealIsNotADuplicate(clean, 0);

            // Create the meal
            var item = new Meal
            {
                Name = clean,
                Portions = portions,
                FoodSourceId = foodSourceId,
                NutritionalValueId = nutritionalValueId
            };

            // Add it to the database
            await Context.Meals.AddAsync(item);
            await Context.SaveChangesAsync();

            return item;
        }

        /// <summary>
        /// Update the properties of the specified meal
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="portion"></param>
        /// <param name="foodSourceId"></param>
        /// <param name="nutritionalValueId"></param>
        /// <returns></returns>
        public async Task<Meal> UpdateAsync(int id, string name, int portions, int foodSourceId, int? nutritionalValueId)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Updating meal with ID {id}: Name = {name}, Portions = {portions}, Food Source ID = {foodSourceId}, Nutritional Value ID = {nutritionalValueId}");

            // Retrieve the meal for update
            var item = Context.Meals.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                // Clean up the name and make sure we're not creating a duplicate and that the
                // related nutritional value exists
                var clean = StringCleaner.Clean(name);
                Factory.FoodSources.CheckFoodSourceExists(foodSourceId);
                Factory.NutritionalValues.CheckNutritionalValueExists(nutritionalValueId);
                await CheckMealIsNotADuplicate(clean, id);

                // Update the meal
                item.Name = clean;
                item.Portions = portions;
                item.FoodSourceId = foodSourceId;
                item.NutritionalValueId = nutritionalValueId;
                await Context.SaveChangesAsync();

                // Reload the associated nutritional value and food source
                await Context.Entry(item).Reference(x => x.FoodSource).LoadAsync();
                await Context.Entry(item).Reference(x => x.NutritionalValue).LoadAsync();
            }

            return item;
        }

        /// <summary>
        /// Delete the meal with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Deleting meal with ID {id}");

            var item = Context.Meals.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                // Check the meal isn't referenced in a meal/food item relationship
                var relationship = await Factory.MealFoodItems.GetAsync(x => x.MealId == id);
                if (relationship != null)
                {
                    var message = $"Meal with ID {id} has food item relationships and cannot be deleted";
                    throw new MealInUseException(message);
                }

                // Delete the meal
                Factory.Context.Remove(item);
                await Factory.Context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Check a meal type with the specified ID exists and raise an exception if not
        /// </summary>
        /// <param name="mealId"></param>
        public void CheckMealExists(int mealId)
        {
            var meal = Context.Meals.FirstOrDefault(x => x.Id == mealId);
            if (meal == null)
            {
                var message = $"Meal with Id {mealId} does not exist";
                throw new MealNotFoundException(message);
            }
        }

        /// <summary>
        /// Raise an exception if an attempt is made to add/update a meal with a duplicate
        /// name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <exception cref="MealExistsException"></exception>
        private async Task CheckMealIsNotADuplicate(string name, int id)
        {
            var meal = await Context.Meals.FirstOrDefaultAsync(x => x.Name == name);
            if ((meal != null) && (meal.Id != id))
            {
                var message = $"Meal {name} already exists";
                throw new MealExistsException(message);
            }
        }
    }
}

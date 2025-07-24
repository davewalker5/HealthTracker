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
                            .Include(x => x.MealFoodItems)
                                .ThenInclude(x => x.FoodItem)
                                    .ThenInclude(x => x.FoodCategory)
                            .Include(x => x.MealFoodItems)
                                .ThenInclude(x => x.NutritionalValue)
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
            var meal = new Meal
            {
                Name = clean,
                Portions = portions,
                FoodSourceId = foodSourceId,
                NutritionalValueId = nutritionalValueId
            };

            // Add it to the database
            await Context.Meals.AddAsync(meal);
            await Context.SaveChangesAsync();

            // Reload to load associated entities
            meal = (await ListAsync(x => x.Id == meal.Id, 1, 1)).First();
            return meal;
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
            var meal = Context.Meals.FirstOrDefault(x => x.Id == id);
            if (meal == null)
            {
                var message = $"Meal with ID {id} not found";
                throw new MealNotFoundException(message);
            }

            // Clean up the name and make sure we're not creating a duplicate and that the
            // related nutritional value exists
            var clean = StringCleaner.Clean(name);
            Factory.FoodSources.CheckFoodSourceExists(foodSourceId);
            Factory.NutritionalValues.CheckNutritionalValueExists(nutritionalValueId);
            await CheckMealIsNotADuplicate(clean, id);

            // Update the meal
            meal.Name = clean;
            meal.Portions = portions;
            meal.FoodSourceId = foodSourceId;
            meal.NutritionalValueId = nutritionalValueId;
            await Context.SaveChangesAsync();

            // Reload to load associated entities
            meal = (await ListAsync(x => x.Id == id, 1, 1)).First();
            return meal;
        }

        /// <summary>
        /// Update the total nutritional value for a meal based on the associated food itm
        /// relationships
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="MealNotFoundException"></exception>
        public async Task UpdateNutritionalValues(int id)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Updating nutritional values for meal with ID = {id}");

            // Retrieve the meal
            var meal = Context.Meals.FirstOrDefault(x => x.Id == id);
            if (meal == null)
            {
                var message = $"Meal with ID {id} not found";
                throw new MealNotFoundException(message);
            }

            Factory.Logger.LogMessage(Severity.Debug, $"Retrieved meal {meal}");

            // Retrieve the associated food item relationships and calculate a total nutritional value
            var relationships = await Factory.MealFoodItems.ListAsync(x => x.MealId == id);
            var plural = relationships.Count == 1 ? "" : "s";
            Factory.Logger.LogMessage(Severity.Debug, $"Found {relationships.Count} food item relationship{plural} associated with meal ID {id}");

            var calculated = Factory.NutritionalValues.CalculateTotalNutritionalValue(relationships);
            Factory.Logger.LogMessage(Severity.Debug, $"Calculated nutritional values : {calculated}");

            var nutritionalValues = await Factory.NutritionalValues.CreateOrUpdateNutritionalValueAsync(meal.NutritionalValueId, calculated);
            Factory.Logger.LogMessage(Severity.Debug, $"Created/updated nutritional values : {nutritionalValues}");

            // Update the meal to associate the nutritional values with it, if the calculated values have some
            // values, or remove any existing association if they don't
            if (nutritionalValues != null)
            {
                Factory.Logger.LogMessage(Severity.Debug, $"Setting nutritional value ID for meal {id} to {nutritionalValues.Id}");
                meal.NutritionalValueId = nutritionalValues.Id;
                await Context.SaveChangesAsync();
            }
            else if (meal.NutritionalValueId != null)
            {
                Factory.Logger.LogMessage(Severity.Debug, $"Removing nutritional value ID from meal {id}");
                meal.NutritionalValueId = null;
                await Context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Delete the meal with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Deleting meal with ID {id}");

            var meal = Context.Meals.FirstOrDefault(x => x.Id == id);
            if (meal == null)
            {
                var message = $"Meal with ID {id} not found";
                throw new MealNotFoundException(message);
            }

            // Check the meal isn't referenced in a meal/food item relationship
            var relationship = await Factory.MealFoodItems.GetAsync(x => x.MealId == id);
            if (relationship != null)
            {
                var message = $"Meal with ID {id} has food item relationships and cannot be deleted";
                throw new MealInUseException(message);
            }

            // Delete the meal
            Factory.Context.Remove(meal);
            await Factory.Context.SaveChangesAsync();
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

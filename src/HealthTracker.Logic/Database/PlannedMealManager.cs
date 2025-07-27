using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Logging;
using HealthTracker.Entities.Food;
using HealthTracker.Entities.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.Logic.Database
{
    public class PlannedMealManager : MeasurementManagerBase, IPlannedMealManager
    {
        internal PlannedMealManager(IHealthTrackerFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return the first planned meal matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<PlannedMeal> GetAsync(Expression<Func<PlannedMeal, bool>> predicate)
        {
            var BeverageMeasures = await ListAsync(predicate, 1, int.MaxValue);
            return BeverageMeasures.FirstOrDefault();
        }

        /// <summary>
        /// Return all planned meals matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<PlannedMeal>> ListAsync(Expression<Func<PlannedMeal, bool>> predicate, int pageNumber, int pageSize)
            => await Context.PlannedMeals
                            .Include(x => x.Meal)
                                .ThenInclude(x => x.NutritionalValue)
                            .Include(x => x.Meal)
                                .ThenInclude(x => x.MealFoodItems)
                                    .ThenInclude(x => x.FoodItem)
                                        .ThenInclude(x => x.FoodCategory)
                            .Include(x => x.Meal)
                                .ThenInclude(x => x.MealFoodItems)
                                    .ThenInclude(x => x.NutritionalValue)
                            .Where(predicate)
                            .OrderBy(x => x.Meal.Name)
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();

        /// <summary>
        /// Add a new planned meal
        /// </summary>
        /// <param name="mealType"></param>
        /// <param name="date"></param>
        /// <param name="mealId"></param>
        /// <returns></returns>
        public async Task<PlannedMeal> AddAsync(MealType mealType, DateTime date, int mealId)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Adding planned meal: Type = {mealType}, Date = {date}, Meal ID = {mealId}");

            // Clean up the date and make sure we're not creating a duplicate and that the
            // related meal exists
            var cleanDate = RemoveTimestamp(date);
            Factory.Meals.CheckMealExists(mealId);
            await CheckPlannedMealIsNotADuplicate(mealType, cleanDate, 0);

            // Create the planned meal
            var plannedMeal = new PlannedMeal
            {
                MealType = mealType,
                Date = cleanDate,
                MealId = mealId
            };

            // Add it to the database
            await Context.PlannedMeals.AddAsync(plannedMeal);
            await Context.SaveChangesAsync();

            // Reload to load associated entities
            plannedMeal = await GetAsync(x => x.Id == plannedMeal.Id);
            return plannedMeal;
        }

        /// <summary>
        /// Update the properties of the specified planned meal
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mealType"></param>
        /// <param name="date"></param>
        /// <param name="mealId"></param>
        /// <returns></returns>
        public async Task<PlannedMeal> UpdateAsync(int id, MealType mealType, DateTime date, int mealId)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Updating planned meal with ID {id}: Type = {mealType}, Date = {date}, Meal ID = {mealId}");

            // Retrieve the meal for update
            var plannedMeal = Context.PlannedMeals.FirstOrDefault(x => x.Id == id);
            if (plannedMeal == null)
            {
                var message = $"Planned meal with ID {id} not found";
                throw new PlannedMealNotFoundException(message);
            }

            // Clean up the date and make sure we're not creating a duplicate and that the
            // related meal exists
            var cleanDate = RemoveTimestamp(date);
            Factory.Meals.CheckMealExists(mealId);
            await CheckPlannedMealIsNotADuplicate(mealType, cleanDate, id);

            // Update the meal
            plannedMeal.MealType = mealType;
            plannedMeal.Date = cleanDate;
            plannedMeal.MealId = mealId;
            await Context.SaveChangesAsync();

            // Reload to load associated entities
            plannedMeal = await GetAsync(x => x.Id == plannedMeal.Id);
            return plannedMeal;
        }

        /// <summary>
        /// Delete the planned meal with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Deleting planned meal with ID {id}");

            var meal = Context.PlannedMeals.FirstOrDefault(x => x.Id == id);
            if (meal == null)
            {
                var message = $"Planned meal with ID {id} not found";
                throw new PlannedMealNotFoundException(message);
            }

            // Delete the meal
            Factory.Context.Remove(meal);
            await Factory.Context.SaveChangesAsync();
        }

        /// <summary>
        /// Purge all planned meals with a date less than a specified cutoff, or less than midnight last
        /// night if no cutoff is specified
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task Purge(DateTime? cutoff)
        {
            var cleanCutoff = RemoveTimestamp(cutoff);
            Factory.Logger.LogMessage(Severity.Info, $"Purging planned meals with date < {cleanCutoff}");
            await Context.PlannedMeals.Where(x => x.Date < cleanCutoff).ExecuteDeleteAsync();
        }

        /// <summary>
        /// Raise an exception if an attempt is made to add/update a planned meal with a duplicate type and date
        /// </summary>
        /// <param name="mealType"></param>
        /// <param name="date"></param>
        /// <param name="id"></param>
        /// <exception cref="PlannedMealExistsException"></exception>
        private async Task CheckPlannedMealIsNotADuplicate(MealType mealType, DateTime date, int id)
        {
            var meal = await Context.PlannedMeals.FirstOrDefaultAsync(x => (x.MealType == mealType) && (x.Date == date));
            if ((meal != null) && (meal.Id != id))
            {
                var message = $"Planned meal type {mealType} on {date} already exists";
                throw new PlannedMealExistsException(message);
            }
        }

        /// <summary>
        /// Return a "cleaned" version of a date with the timestamp removed
        /// </summary>
        /// <param name="mealType"></param>
        /// <returns></returns>
        private DateTime RemoveTimestamp(DateTime? date)
        {
            DateTime cleaned;
            if (date != null)
            {
                cleaned = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, 0, 0, 0);
            }
            else
            {
                var today = DateTime.Now;
                cleaned = new DateTime(today.Year, today.Month, today.Day, 0, 0, 0);
            }
            return cleaned;
        }
    }
}

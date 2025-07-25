using System.Diagnostics.CodeAnalysis;
using HealthTracker.Client.Interfaces;
using HealthTracker.Entities.Food;
using Microsoft.Extensions.Logging;

namespace HealthTracker.Client.Helpers
{
    [ExcludeFromCodeCoverage]
    public class MealHelper : NutritionalValueHelper<MealHelper>, IMealHelper
    {
        private readonly IMealClient _mealClient;

        public MealHelper(IMealClient mealClient, INutritionalValueClient nutritionalValueClient, ILogger<MealHelper> logger)
            : base(nutritionalValueClient, logger)
        {
            _mealClient = mealClient;
        }

        /// <summary>
        /// Convenience wrapper to get a single meal given its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Meal> GetAsync(int id)
            => await _mealClient.GetAsync(id);

        /// <summary>
        /// Convenience wrapper to list existing meals
        /// </summary>
        /// <param name="foodCategoryId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Meal>> ListAsync(int foodCategoryId, int pageNumber, int pageSize)
            => await _mealClient.ListAsync(foodCategoryId, pageNumber, pageSize);

        /// <summary>
        /// Convenience wrapper to delete an existing meal and associated nutritional values
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
            => await _mealClient.DeleteAsync(id);

        /// <summary>
        /// Add a new meal with associated nutritional value
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public async Task<Meal> AddAsync(Meal template)
        {
            Logger.LogDebug(
                $"Adding new food item: Food Source ID = {template.FoodSourceId}, " +
                $"Name = {template.Name}, " +
                $"Portions = {template.Portions}");

            // Add the meal without an associated nutritional value, to begin with. This will flush out errors generated by
            // adding the meal itself and won't create redundant nutritional value records
            var meal = await _mealClient.AddAsync(template.Name, template.Portions, template.FoodSourceId, null);

            // Meal's now been added successfully, so we can add the nutritional value
            var nutritionalValueId = await AddNutritionalValue(template.NutritionalValue);

            // And, finally, associate the nutritional value record with the meal
            meal = await _mealClient.UpdateAsync(meal.Id, meal.Name, meal.Portions, meal.FoodSourceId, nutritionalValueId);

            return meal;
        }

        /// <summary>
        /// Update an existing meal and its associated nutritional values
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public async Task<Meal> UpdateAsync(Meal template)
        {
            Logger.LogDebug(
                $"Updating food item: ID = {template.Id}, " +
                $"Food Source ID = {template.FoodSourceId}, " +
                $"Name = {template.Name}, " +
                $"Portions = {template.Portions}");

            // update the meal without an associated nutritional value, to begin with. This will flush out errors generated by
            // updating the meal itself and won't create redundant nutritional value records
            var meal = await _mealClient.UpdateAsync(template.Id, template.Name, template.Portions, template.FoodSourceId, null);

            Logger.LogDebug($"Template meal has Nutritional Value ID = {template.NutritionalValueId}");
            Logger.LogDebug($"Template nutritional values 'HasValues' = {template.NutritionalValue?.HasValues}");

            int? nutritionalValueId = null;
            if (((template.NutritionalValueId ?? 0) <= 0) && (template.NutritionalValue?.HasValues == true))
            {
                // New nutritional value record and it has values associated with it : Save it
                nutritionalValueId = await AddNutritionalValue(template.NutritionalValue);
            }
            else if (template.NutritionalValueId > 0)
            {
                // Existing nutritional value record : Update it or delete it, if it no longer has values
                nutritionalValueId = await UpdateNutritionalValueAsync(template.NutritionalValueId, template.NutritionalValue);
            }

            // If we still have a nutritional value, re-associate it with the meal
            if (nutritionalValueId > 0)
            {
                meal = await _mealClient.UpdateAsync(meal.Id, meal.Name, meal.Portions, meal.FoodSourceId, nutritionalValueId);
            }

            return meal;
        }
    }
}
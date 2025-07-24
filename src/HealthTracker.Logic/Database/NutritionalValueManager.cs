using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Logging;
using HealthTracker.Entities.Food;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HealthTracker.Logic.Database
{
    public class NutritionalValueManager : DatabaseManagerBase, INutritionalValueManager
    {
        internal NutritionalValueManager(IHealthTrackerFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return the first set of nutritional values matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<NutritionalValue> GetAsync(Expression<Func<NutritionalValue, bool>> predicate)
            => await Context.NutritionalValues
                            .Where(predicate)
                            .FirstOrDefaultAsync();

        /// <summary>
        /// Add a set of nutritional values
        /// </summary>
        /// <param name="calories"></param>
        /// <param name="fat"></param>
        /// <param name="saturatedFat"></param>
        /// <param name="protein"></param>
        /// <param name="carbohydrates"></param>
        /// <param name="sugar"></param>
        /// <param name="fibre"></param>
        /// <returns></returns>
        public async Task<NutritionalValue> AddAsync(
            decimal? calories,
            decimal? fat,
            decimal? saturatedFat,
            decimal? protein,
            decimal? carbohydrates,
            decimal? sugar,
            decimal? fibre)
        {
            Factory.Logger.LogMessage(Severity.Info,
                $"Creating new nutritional values : " +
                $"Calories = {calories}, " +
                $"Fat = {fat}, " +
                $"Saturated Fat = {saturatedFat}, " +
                $"Protein = {protein}, " +
                $"Carbohydrates = {carbohydrates}, " +
                $"Sugar = {sugar}, " +
                $"Fibre = {fibre}");

            var nutrition = new NutritionalValue
            {
                Calories = calories,
                Fat = fat,
                SaturatedFat = saturatedFat,
                Protein = protein,
                Carbohydrates = carbohydrates,
                Sugar = sugar,
                Fibre = fibre
            };

            await Context.NutritionalValues.AddAsync(nutrition);
            await Context.SaveChangesAsync();

            return nutrition;
        }

        /// <summary>
        /// Add a new set of nutritional values from an instance
        /// </summary>
        /// <param name="nutrition"></param>
        /// <returns></returns>
        public async Task<NutritionalValue> AddAsync(NutritionalValue nutrition)
        {
            await Context.NutritionalValues.AddAsync(nutrition);
            await Context.SaveChangesAsync();
            return nutrition;
        }

        /// <summary>
        /// Update the properties of the specified set of nutritional values
        /// </summary>
        /// <param name="id"></param>
        /// <param name="calories"></param>
        /// <param name="fat"></param>
        /// <param name="saturatedFat"></param>
        /// <param name="protein"></param>
        /// <param name="carbohydrates"></param>
        /// <param name="sugar"></param>
        /// <param name="fibre"></param>
        /// <returns></returns>
        public async Task<NutritionalValue> UpdateAsync(
            int id,
            decimal? calories,
            decimal? fat,
            decimal? saturatedFat,
            decimal? protein,
            decimal? carbohydrates,
            decimal? sugar,
            decimal? fibre)
        {
            Factory.Logger.LogMessage(Severity.Info,
                $"Updating nutritional values with ID {id} : " +
                $"Calories = {calories}, " +
                $"Fat = {fat}, " +
                $"Saturated Fat = {saturatedFat}, " +
                $"Protein = {protein}, " +
                $"Carbohydrates = {carbohydrates}, " +
                $"Sugar = {sugar}, " +
                $"Fibre = {fibre}");

            var nutrition = Context.NutritionalValues.FirstOrDefault(x => x.Id == id);
            if (nutrition != null)
            {
                nutrition.Calories = calories;
                nutrition.Fat = fat;
                nutrition.SaturatedFat = saturatedFat;
                nutrition.Protein = protein;
                nutrition.Carbohydrates = carbohydrates;
                nutrition.Sugar = sugar;
                nutrition.Fibre = fibre;
                await Context.SaveChangesAsync();
            }

            return nutrition;
        }

        /// <summary>
        /// Update an existing nutritional value from the supplied template
        /// </summary>
        /// <param name="id"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        public async Task<NutritionalValue> CreateOrUpdateNutritionalValueAsync(int? id, NutritionalValue template)
        {
            NutritionalValue values = null;

            var isExistingId = id > 0;
            var updateHasValues = template?.HasValues == true;

            // Assess the state of the current record (based on the ID) and the update provided
            // in the template:
            //
            // 1. New Record, update has values => Create it
            // 2. Existing record, update has values => Update it
            // 3. Existing record, update does not have values => Delete it
            // 4. New record, update does not have values => No further action
            //
            if (!isExistingId && updateHasValues)
            {
                values = await AddAsync(
                    template.Calories,
                    template.Fat,
                    template.SaturatedFat,
                    template.Protein,
                    template.Carbohydrates,
                    template.Sugar,
                    template.Fibre);
            }
            else if (isExistingId && updateHasValues)
            {
                values = await UpdateAsync(
                    id.Value,
                    template.Calories,
                    template.Fat,
                    template.SaturatedFat,
                    template.Protein,
                    template.Carbohydrates,
                    template.Sugar,
                    template.Fibre);
            }
            else if (isExistingId && !updateHasValues)
            {
                await DeleteAsync(id.Value);
            }

            return values;
        }

        /// <summary>
        /// Delete the nutritional values with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Deleting nutritional values with ID {id}");

            var nutrition = await GetAsync(x => x.Id == id);
            if (nutrition != null)
            {
                Factory.Context.Remove(nutrition);
                await Factory.Context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Check a nutritional vaue with a specified ID exists and raise an exception if not
        /// </summary>
        /// <param name="nutritionalValueId"></param>
        public void CheckNutritionalValueExists(int? nutritionalValueId)
        {
            if (nutritionalValueId != null)
            {
                var nutrition = Context.NutritionalValues.FirstOrDefault(x => x.Id == nutritionalValueId);
                if (nutrition == null)
                {
                    var message = $"Nutritional value with Id {nutritionalValueId} does not exist";
                    throw new NutritionalValueNotFoundException(message);
                }
            }
        }

        /// <summary>
        /// Calculate a set of nutritional values given a base set of values and a quantity
        /// </summary>
        /// <param name="baseNutrition"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public NutritionalValue CalculateNutritionalValues(NutritionalValue baseNutrition, decimal quantity)
        {
            NutritionalValue calculated = null;

            // Only return something if the specified base values have at least one property set
            if (baseNutrition?.HasValues == true)
            {
                calculated = new()
                {
                    Calories = CalculateValue(baseNutrition.Calories, quantity),
                    Fat = CalculateValue(baseNutrition.Fat, quantity),
                    SaturatedFat = CalculateValue(baseNutrition.SaturatedFat, quantity),
                    Protein = CalculateValue(baseNutrition.Protein, quantity),
                    Carbohydrates = CalculateValue(baseNutrition.Carbohydrates, quantity),
                    Sugar = CalculateValue(baseNutrition.Sugar, quantity),
                    Fibre = CalculateValue(baseNutrition.Fibre, quantity)
                };
            }

            return calculated;
        }

        /// <summary>
        /// Calculate the total nutrition for a collection of meal/food item relationships
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public NutritionalValue CalculateTotalNutritionalValue(IEnumerable<MealFoodItem> items)
        {
            NutritionalValue calculated = null;

            // Get a collection of nutritional values from the meal/food item relationships
            var nutritionalValues = items?.Select(x => x.NutritionalValue);

            // Only return something if the specified base values have at least one property set
            if (nutritionalValues?.Count() > 0)
            {
                calculated = new()
                {
                    Calories = nutritionalValues.Where(x => x.Calories.HasValue).Sum(x => x.Calories),
                    Fat = nutritionalValues.Where(x => x.Fat.HasValue).Sum(x => x.Fat),
                    SaturatedFat = nutritionalValues.Where(x => x.SaturatedFat.HasValue).Sum(x => x.SaturatedFat),
                    Protein = nutritionalValues.Where(x => x.Protein.HasValue).Sum(x => x.Protein),
                    Carbohydrates = nutritionalValues.Where(x => x.Carbohydrates.HasValue).Sum(x => x.Carbohydrates),
                    Sugar = nutritionalValues.Where(x => x.Sugar.HasValue).Sum(x => x.Sugar),
                    Fibre = nutritionalValues.Where(x => x.Fibre.HasValue).Sum(x => x.Fibre)
                };
            }

            return calculated?.HasValues == true ? calculated : null;
        }

        /// <summary>
        /// Calculate a nullable nutritional value from the base value and a quantity
        /// </summary>
        /// <param name="value"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        private decimal? CalculateValue(decimal? value, decimal quantity)
            => value == null ? null : Math.Round(value.Value * quantity, 4);
    }
}

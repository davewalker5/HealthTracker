using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Logging;
using HealthTracker.Entities.Food;
using HealthTracker.Logic.Extensions;
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
                $"Updatind nutritional values with ID {id} : " +
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
    }
}

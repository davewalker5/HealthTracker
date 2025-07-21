using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Logging;
using HealthTracker.Entities.Food;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HealthTracker.Logic.Database
{
    public class MealConsumptionMeasurementManager : MeasurementManagerBase, IMealConsumptionMeasurementManager
    {
        internal MealConsumptionMeasurementManager(IHealthTrackerFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return all measurements matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<MealConsumptionMeasurement>> ListAsync(Expression<Func<MealConsumptionMeasurement, bool>> predicate, int pageNumber, int pageSize)
            => await Context.MealConsumptionMeasurements
                            .Include(x => x.NutritionalValue)
                            .Include(x => x.Meal)
                            .ThenInclude(x => x.NutritionalValue)
                            .Where(predicate)
                            .OrderBy(x => x.Date)
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();

        /// <summary>
        /// Add a new measurement
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="mealId"></param>
        /// <param name="nutritionalValueId"></param>
        /// <param name="date"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public async Task<MealConsumptionMeasurement> AddAsync(
            int personId,
            int mealId,
            int? nutritionalValueId,
            DateTime date,
            decimal quantity)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Adding meal consumption measurement: Person ID {personId}, {date.ToShortDateString()}, Meal ID {mealId}, Quantity {quantity}, Nutritional Value ID = {nutritionalValueId}");

            CheckPersonExists(personId);
            Factory.Meals.CheckMealExists(mealId);

            var measurement = new MealConsumptionMeasurement
            {
                PersonId = personId,
                MealId = mealId,
                Date = date,
                Quantity = quantity,
                NutritionalValueId = nutritionalValueId
            };

            await Context.MealConsumptionMeasurements.AddAsync(measurement);
            await Context.SaveChangesAsync();

            return measurement;
        }

        /// <summary>
        /// Update the properties of the specified measurement
        /// </summary>
        /// <param name="id"></param>
        /// <param name="personId"></param>
        /// <param name="mealId"></param>
        /// <param name="nutritionalValueId"></param>
        /// <param name="date"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public async Task<MealConsumptionMeasurement> UpdateAsync(
            int id,
            int personId,
            int mealId,
            int? nutritionalValueId,
            DateTime date,
            decimal quantity)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Updating meal consumption measurement with ID {id}: Person ID {personId}, {date.ToShortDateString()}, Meal ID {mealId}, Nutritional Value ID = {nutritionalValueId}");

            var measurement = Context.MealConsumptionMeasurements.FirstOrDefault(x => x.Id == id);
            if (measurement != null)
            {
                CheckPersonExists(personId);
                Factory.Meals.CheckMealExists(mealId);

                // Save the changes
                measurement.PersonId = personId;
                measurement.MealId = mealId;
                measurement.Date = date;
                measurement.Quantity = quantity;
                measurement.NutritionalValueId = nutritionalValueId;
                await Context.SaveChangesAsync();

                // Reload the associated meal and nutritional value
                await Context.Entry(measurement).Reference(x => x.Meal).LoadAsync();
                await Context.Entry(measurement).Reference(x => x.NutritionalValue).LoadAsync();
            }

            return measurement;
        }

        /// <summary>
        /// Delete the measurement with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Deleting meal consumption measurement with ID {id}");

            var measurement = Context.MealConsumptionMeasurements.FirstOrDefault(x => x.Id == id);
            if (measurement != null)
            {
                Factory.Context.Remove(measurement);
                await Factory.Context.SaveChangesAsync();
            }
        }
    }
}
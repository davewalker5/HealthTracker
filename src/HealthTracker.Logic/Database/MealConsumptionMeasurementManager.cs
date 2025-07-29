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
        /// <param name="date"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public async Task<MealConsumptionMeasurement> AddAsync(
            int personId,
            int mealId,
            DateTime date,
            decimal quantity)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Adding meal consumption measurement: Person ID {personId}, {date.ToShortDateString()}, Meal ID {mealId}, Quantity {quantity}");

            // Check the required entities exist
            CheckPersonExists(personId);
            Factory.Meals.CheckMealExists(mealId);

            // Create and save the measurement
            var measurement = new MealConsumptionMeasurement
            {
                PersonId = personId,
                MealId = mealId,
                Date = date,
                Quantity = quantity
            };

            await Context.MealConsumptionMeasurements.AddAsync(measurement);
            await Context.SaveChangesAsync();

            // Create or update the nutritional value for this record
            await UpdateNutritionalValues(measurement.Id);

            // Reload to load related entities
            measurement = (await ListAsync(x => x.Id == measurement.Id, 1, 1)).First();
            return measurement;
        }

        /// <summary>
        /// Update the properties of the specified measurement
        /// </summary>
        /// <param name="id"></param>
        /// <param name="personId"></param>
        /// <param name="mealId"></param>
        /// <param name="date"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public async Task<MealConsumptionMeasurement> UpdateAsync(
            int id,
            int personId,
            int mealId,
            DateTime date,
            decimal quantity)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Updating meal consumption measurement with ID {id}: Person ID {personId}, {date.ToShortDateString()}, Meal ID {mealId}");

            var measurement = Context.MealConsumptionMeasurements.FirstOrDefault(x => x.Id == id);
            if (measurement == null)
            {
                var message = $"Meal consumption record with ID = {id} not found";
                throw new MealConsumptionMeasurementNotFoundException(message);
            }

            CheckPersonExists(personId);
            Factory.Meals.CheckMealExists(mealId);

            // Save the changes
            measurement.PersonId = personId;
            measurement.MealId = mealId;
            measurement.Date = date;
            measurement.Quantity = quantity;
            await Context.SaveChangesAsync();

            // Create or update the nutritional value for this record
            await UpdateNutritionalValues(measurement.Id);

            // Reload to load related entities
            measurement = (await ListAsync(x => x.Id == measurement.Id, 1, 1)).First();
            return measurement;
        }

        /// <summary>
        /// Update the nutritional value for a consumption record
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="MealNotFoundException"></exception>
        public async Task UpdateNutritionalValues(int id)
        {
            // Load the measurement
            var measurement = Context.MealConsumptionMeasurements.FirstOrDefault(x => x.Id == id);
            if (measurement == null)
            {
                var message = $"Meal consumption record with ID = {id} not found";
                throw new MealConsumptionMeasurementNotFoundException(message);
            }

            // Create the nutritional value for this consumption record
            var meal = (await Factory.Meals.ListAsync(x => x.Id == measurement.MealId, 1, 1)).First();
            var multiplier = measurement.Quantity / (decimal)meal.Portions;
            var calculated = Factory.NutritionalValues.CalculateNutritionalValues(meal.NutritionalValue, multiplier);
            var nutritionalValues = await Factory.NutritionalValues.CreateOrUpdateNutritionalValueAsync(measurement.NutritionalValueId, calculated);

            // Update the measurement to associate the nutritional values with it
            if (nutritionalValues != null)
            {
                measurement.NutritionalValueId = nutritionalValues.Id;
                await Context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Update the nutritional value for all meal consumption records in the database
        /// </summary>
        /// <returns></returns>
        public async Task UpdateAllNutritionalValues()
        {
            Factory.Logger.LogMessage(Severity.Info, $"Updating nutritional values for all meal consumption records");
            var measurementIds = await Context.MealConsumptionMeasurements.Select(x => x.Id).ToListAsync();
            foreach (var id in measurementIds)
            {
                await UpdateNutritionalValues(id);
            }
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
            if (measurement == null)
            {
                var message = $"Meal consumption record with ID = {id} not found";
                throw new MealConsumptionMeasurementNotFoundException(message);
            }

            Factory.Context.Remove(measurement);
            await Factory.Context.SaveChangesAsync();
        }
    }
}
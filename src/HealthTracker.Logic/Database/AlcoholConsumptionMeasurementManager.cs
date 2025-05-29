using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Logging;
using HealthTracker.Entities.Measurements;
using HealthTracker.Enumerations.Enumerations;
using HealthTracker.Logic.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HealthTracker.Logic.Database
{
    public class AlcoholConsumptionMeasurementManager : MeasurementManagerBase, IAlcoholConsumptionMeasurementManager
    {
        internal AlcoholConsumptionMeasurementManager(IHealthTrackerFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return all measurements matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<AlcoholConsumptionMeasurement>> ListAsync(Expression<Func<AlcoholConsumptionMeasurement, bool>> predicate, int pageNumber, int pageSize)
            => await Context.AlcoholConsumptionMeasurements
                            .Where(predicate)
                            .OrderBy(x => x.Date)
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();

        /// <summary>
        /// Add a new measurement
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="beverageId"></param>
        /// <param name="date"></param>
        /// <param name="measure"></param>
        /// <param name="quantity"></param>
        /// <param name="abv"></param>
        /// <returns></returns>
        public async Task<AlcoholConsumptionMeasurement> AddAsync(
            int personId,
            int beverageId,
            DateTime date,
            AlcoholMeasure measure,
            int quantity,
            decimal abv)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Adding alcohol consumption measurement: Person ID {personId}, {date.ToShortDateString()}, Beverage ID {beverageId}, Quantity {quantity}, ABV {abv}");

            CheckPersonExists(personId);
            CheckBeverageExists(beverageId);

            var measurement = new AlcoholConsumptionMeasurement
            {
                PersonId = personId,
                BeverageId = beverageId,
                Date = date,
                Measure = measure,
                Quantity = quantity,
                ABV = abv
            };

            await Context.AlcoholConsumptionMeasurements.AddAsync(measurement);
            await Context.SaveChangesAsync();

            return measurement;
        }

        /// <summary>
        /// Update the properties of the specified measurement
        /// </summary>
        /// <param name="id"></param>
        /// <param name="personId"></param>
        /// <param name="beverageId"></param>
        /// <param name="date"></param>
        /// <param name="measure"></param>
        /// <param name="quantity"></param>
        /// <param name="abv"></param>
        /// <returns></returns>
        public async Task<AlcoholConsumptionMeasurement> UpdateAsync(
            int id,
            int personId,
            int beverageId,
            DateTime date,
            AlcoholMeasure measure,
            int quantity,
            decimal abv)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Updating alcohol consumption measurement with ID {id}: Person ID {personId}, {date.ToShortDateString()}, Beverage ID {beverageId}, Quantity {quantity}, ABV {abv}");

            var measurement = Context.AlcoholConsumptionMeasurements.FirstOrDefault(x => x.Id == id);
            if (measurement != null)
            {
                CheckPersonExists(personId);
                CheckBeverageExists(beverageId);

                // Save the changes
                measurement.PersonId = personId;
                measurement.BeverageId = beverageId;
                measurement.Date = date;
                measurement.Measure = measure;
                measurement.Quantity = quantity;
                measurement.ABV = abv;
                await Context.SaveChangesAsync();
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
            Factory.Logger.LogMessage(Severity.Info, $"Deleting alcohol consumption measurement with ID {id}");

            var measurement = Context.AlcoholConsumptionMeasurements.FirstOrDefault(x => x.Id == id);
            if (measurement != null)
            {
                Factory.Context.Remove(measurement);
                await Factory.Context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Check an activity type with a specified ID exists and raise an exception if not
        /// </summary>
        /// <param name="beverageId"></param>
        private void CheckBeverageExists(int beverageId)
        {
            var beverage = Context.Beverages.FirstOrDefault(x => x.Id == beverageId);
            if (beverage == null)
            {
                var message = $"Beverage with Id {beverageId} does not exist";
                throw new BeverageNotFoundException(message);
            }
        }
    }
}
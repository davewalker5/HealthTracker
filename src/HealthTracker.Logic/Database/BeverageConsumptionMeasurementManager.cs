using HealthTracker.Entities.Exceptions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Logging;
using HealthTracker.Entities.Measurements;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HealthTracker.Logic.Database
{
    public class BeverageConsumptionMeasurementManager : MeasurementManagerBase, IBeverageConsumptionMeasurementManager
    {
        internal BeverageConsumptionMeasurementManager(IHealthTrackerFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return all measurements matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<BeverageConsumptionMeasurement>> ListAsync(Expression<Func<BeverageConsumptionMeasurement, bool>> predicate, int pageNumber, int pageSize)
            => await Context.BeverageConsumptionMeasurements
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
        /// <param name="quantity"></param>
        /// <param name="volume"></param>
        /// <param name="abv"></param>
        /// <returns></returns>
        public async Task<BeverageConsumptionMeasurement> AddAsync(
            int personId,
            int beverageId,
            DateTime date,
            int quantity,
            decimal volume,
            decimal abv)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Adding beverage consumption measurement: Person ID {personId}, {date.ToShortDateString()}, Beverage ID {beverageId}, Quantity {quantity}, Volume {volume}, ABV {abv}");

            CheckPersonExists(personId);
            CheckBeverageExists(beverageId);

            var measurement = new BeverageConsumptionMeasurement
            {
                PersonId = personId,
                BeverageId = beverageId,
                Date = date,
                Quantity = quantity,
                Volume = volume,
                ABV = abv
            };

            await Context.BeverageConsumptionMeasurements.AddAsync(measurement);
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
        /// <param name="quantity"></param>
        /// <param name="volume"></param>
        /// <param name="abv"></param>
        /// <returns></returns>
        public async Task<BeverageConsumptionMeasurement> UpdateAsync(
            int id,
            int personId,
            int beverageId,
            DateTime date,
            int quantity,
            decimal volume,
            decimal abv)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Updating beverage consumption measurement with ID {id}: Person ID {personId}, {date.ToShortDateString()}, Beverage ID {beverageId}, Quantity {quantity}, Volume {volume}, ABV {abv}");

            var measurement = Context.BeverageConsumptionMeasurements.FirstOrDefault(x => x.Id == id);
            if (measurement != null)
            {
                CheckPersonExists(personId);
                CheckBeverageExists(beverageId);

                // Save the changes
                measurement.PersonId = personId;
                measurement.BeverageId = beverageId;
                measurement.Date = date;
                measurement.Quantity = quantity;
                measurement.Volume = volume;
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
            Factory.Logger.LogMessage(Severity.Info, $"Deleting beverage consumption measurement with ID {id}");

            var measurement = Context.BeverageConsumptionMeasurements.FirstOrDefault(x => x.Id == id);
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
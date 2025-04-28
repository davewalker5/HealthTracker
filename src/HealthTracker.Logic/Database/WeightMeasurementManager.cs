using Microsoft.EntityFrameworkCore;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using System.Linq.Expressions;
using HealthTracker.Entities.Logging;

namespace HealthTracker.Logic.Database
{
    public class WeightMeasurementManager : MeasurementManagerBase, IWeightMeasurementManager
    {
        internal WeightMeasurementManager(IHealthTrackerFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return all measurements matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<List<WeightMeasurement>> ListAsync(Expression<Func<WeightMeasurement, bool>> predicate)
            => await Context.WeightMeasurements
                            .Where(predicate)
                            .OrderBy(x => x.Date)
                            .ToListAsync();

        /// <summary>
        /// Add a new measurement
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="date"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public async Task<WeightMeasurement> AddAsync(int personId, DateTime date, decimal weight)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Adding weight measurement: Person ID {personId}, Date {date.ToShortDateString()}, Weight: {weight:.00}");

            CheckPersonExists(personId);

            var measurement = new WeightMeasurement
            {
                PersonId = personId,
                Date = date,
                Weight = weight
            };

            await Context.WeightMeasurements.AddAsync(measurement);
            await Context.SaveChangesAsync();

            return measurement;
        }

        /// <summary>
        /// Update the properties of the specified measurement
        /// </summary>
        /// <param name="id"></param>
        /// <param name="personId"></param>
        /// <param name="date"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public async Task<WeightMeasurement> UpdateAsync(int id, int personId, DateTime date, decimal weight)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Updating weight measurement with ID {id}: Person ID {personId}, Date {date.ToShortDateString()}, Weight: {weight:.00}");

            var measurement = Context.WeightMeasurements.FirstOrDefault(x => x.Id == id);
            if (measurement != null)
            {
                CheckPersonExists(personId);

                // Save the changes
                measurement.PersonId = personId;
                measurement.Date = date;
                measurement.Weight = weight;
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
            Factory.Logger.LogMessage(Severity.Info, $"Deleting weight measurement with ID {id}");

            var measurement = Context.WeightMeasurements.FirstOrDefault(x => x.Id == id);
            if (measurement != null)
            {
                Factory.Context.Remove(measurement);
                await Factory.Context.SaveChangesAsync();
            }
        }
    }
}
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Logging;
using HealthTracker.Entities.Measurements;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HealthTracker.Logic.Database
{
    public class BloodGlucoseMeasurementManager : MeasurementManagerBase, IBloodGlucoseMeasurementManager
    {
        internal BloodGlucoseMeasurementManager(IHealthTrackerFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return all measurements matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<BloodGlucoseMeasurement>> ListAsync(Expression<Func<BloodGlucoseMeasurement, bool>> predicate, int pageNumber, int pageSize)
            => await Context.BloodGlucoseMeasurements
                            .Where(predicate)
                            .OrderBy(x => x.Date)
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();

        /// <summary>
        /// Add a new measurement from individual measurement components
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="date"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public async Task<BloodGlucoseMeasurement> AddAsync(int personId, DateTime date, decimal level)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Adding blood glucose measurement: Person ID {personId}, {date.ToShortDateString()}, {level}");

            CheckPersonExists(personId);

            var measurement = new BloodGlucoseMeasurement
            {
                PersonId = personId,
                Date = date,
                Level = level
            };

            await Context.BloodGlucoseMeasurements.AddAsync(measurement);
            await Context.SaveChangesAsync();

            return measurement;
        }

        /// <summary>
        /// Update the properties of the specified measurement
        /// </summary>
        /// <param name="id"></param>
        /// <param name="personId"></param>
        /// <param name="date"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public async Task<BloodGlucoseMeasurement> UpdateAsync(int id, int personId, DateTime date, decimal level)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Updating blood glucose measurement with ID {id}: Person ID {personId}, {date.ToShortDateString()}, {level}");

            var measurement = Context.BloodGlucoseMeasurements.FirstOrDefault(x => x.Id == id);
            if (measurement != null)
            {
                CheckPersonExists(personId);

                // Save the changes
                measurement.PersonId = personId;
                measurement.Date = date;
                measurement.Level = level;
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
            Factory.Logger.LogMessage(Severity.Info, $"Deleting blood glucose measurement with ID {id}");

            var measurement = Context.BloodGlucoseMeasurements.FirstOrDefault(x => x.Id == id);
            if (measurement != null)
            {
                Factory.Context.Remove(measurement);
                await Factory.Context.SaveChangesAsync();
            }
        }
    }
}

using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Logging;
using HealthTracker.Entities.Measurements;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HealthTracker.Logic.Database
{
    public class BloodPressureMeasurementManager : MeasurementManagerBase, IBloodPressureMeasurementManager
    {
        internal BloodPressureMeasurementManager(IHealthTrackerFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return all measurements matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<List<BloodPressureMeasurement>> ListAsync(Expression<Func<BloodPressureMeasurement, bool>> predicate)
            => await Context.BloodPressureMeasurements
                            .Where(predicate)
                            .OrderBy(x => x.Date)
                            .ToListAsync();

        /// <summary>
        /// Add a new measurement from individual measurement components
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="date"></param>
        /// <param name="systolic"></param>
        /// <param name="diastolic"></param>
        /// <returns></returns>
        public async Task<BloodPressureMeasurement> AddAsync(int personId, DateTime date, int systolic, int diastolic)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Adding blood pressure measurement: Person ID {personId}, {date.ToShortDateString()}, {systolic}/{diastolic}");

            CheckPersonExists(personId);

            var measurement = new BloodPressureMeasurement
            {
                PersonId = personId,
                Date = date,
                Systolic = systolic,
                Diastolic = diastolic
            };

            await Context.BloodPressureMeasurements.AddAsync(measurement);
            await Context.SaveChangesAsync();

            return measurement;
        }

        /// <summary>
        /// Update the properties of the specified measurement
        /// </summary>
        /// <param name="id"></param>
        /// <param name="personId"></param>
        /// <param name="date"></param>
        /// <param name="systolic"></param>
        /// <param name="diastolic"></param>
        /// <returns></returns>
        public async Task<BloodPressureMeasurement> UpdateAsync(int id, int personId, DateTime date, int systolic, int diastolic)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Updating blood pressure measurement with ID {id}: Person ID {personId}, {date.ToShortDateString()}, {systolic}/{diastolic}");

            var measurement = Context.BloodPressureMeasurements.FirstOrDefault(x => x.Id == id);
            if (measurement != null)
            {
                CheckPersonExists(personId);

                // Save the changes
                measurement.PersonId = personId;
                measurement.Date = date;
                measurement.Systolic = systolic;
                measurement.Diastolic = diastolic;
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
            Factory.Logger.LogMessage(Severity.Info, $"Deleting blood pressure measurement with ID {id}");

            var measurement = Context.BloodPressureMeasurements.FirstOrDefault(x => x.Id == id);
            if (measurement != null)
            {
                Factory.Context.Remove(measurement);
                await Factory.Context.SaveChangesAsync();
            }
        }
    }
}

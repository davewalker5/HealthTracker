using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Logging;
using HealthTracker.Entities.Measurements;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HealthTracker.Logic.Database
{
    public class BloodOxygenSaturationMeasurementManager : MeasurementManagerBase, IBloodOxygenSaturationMeasurementManager
    {
        internal BloodOxygenSaturationMeasurementManager(IHealthTrackerFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return all measurements matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<List<BloodOxygenSaturationMeasurement>> ListAsync(Expression<Func<BloodOxygenSaturationMeasurement, bool>> predicate)
            => await Context.BloodOxygenSaturationMeasurements
                            .Where(predicate)
                            .OrderBy(x => x.Date)
                            .ToListAsync();

        /// <summary>
        /// Add a new measurement from individual measurement components
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="date"></param>
        /// <param name="spo2"></param>
        /// <returns></returns>
        public async Task<BloodOxygenSaturationMeasurement> AddAsync(int personId, DateTime date, decimal spo2)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Adding % SPO2 measurement: Person ID {personId}, {date.ToShortDateString()}, {spo2}");

            CheckPersonExists(personId);

            var measurement = new BloodOxygenSaturationMeasurement
            {
                PersonId = personId,
                Date = date,
                Percentage = spo2
            };

            await Context.BloodOxygenSaturationMeasurements.AddAsync(measurement);
            await Context.SaveChangesAsync();

            return measurement;
        }

        /// <summary>
        /// Update the properties of the specified measurement
        /// </summary>
        /// <param name="id"></param>
        /// <param name="personId"></param>
        /// <param name="date"></param>
        /// <param name="spo2"></param>
        /// <returns></returns>
        public async Task<BloodOxygenSaturationMeasurement> UpdateAsync(int id, int personId, DateTime date, decimal spo2)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Updating % SPO2 measurement with ID {id}: Person ID {personId}, {date.ToShortDateString()}, {spo2}");

            var measurement = Context.BloodOxygenSaturationMeasurements.FirstOrDefault(x => x.Id == id);
            if (measurement != null)
            {
                CheckPersonExists(personId);

                // Save the changes
                measurement.PersonId = personId;
                measurement.Date = date;
                measurement.Percentage = spo2;
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
            Factory.Logger.LogMessage(Severity.Info, $"Deleting % SPO2 measurement with ID {id}");

            var measurement = Context.BloodOxygenSaturationMeasurements.FirstOrDefault(x => x.Id == id);
            if (measurement != null)
            {
                Factory.Context.Remove(measurement);
                await Factory.Context.SaveChangesAsync();
            }
        }
    }
}

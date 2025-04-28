using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Logging;
using HealthTracker.Entities.Measurements;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HealthTracker.Logic.Database
{
    public class CholesterolMeasurementManager : MeasurementManagerBase, ICholesterolMeasurementManager
    {
        internal CholesterolMeasurementManager(IHealthTrackerFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return all measurements matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<List<CholesterolMeasurement>> ListAsync(Expression<Func<CholesterolMeasurement, bool>> predicate)
            => await Context.CholesterolMeasurements
                            .Where(predicate)
                            .OrderBy(x => x.Date)
                            .ToListAsync();

        /// <summary>
        /// Add a new measurement from individual measurement components
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="date"></param>
        /// <param name="total"></param>
        /// <param name="hdl"></param>
        /// <param name="ldl"></param>
        /// <param name="triglycerides"></param>
        /// <returns></returns>
        public async Task<CholesterolMeasurement> AddAsync(int personId, DateTime date, decimal total, decimal hdl, decimal ldl, decimal triglycerides)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Adding cholesterol measurement: Person ID {personId}, {date.ToShortDateString()}, Total {total}, HDL {hdl}, LDL {ldl}, Triclycerides {triglycerides}");

            CheckPersonExists(personId);

            var measurement = new CholesterolMeasurement
            {
                PersonId = personId,
                Date = date,
                Total = total,
                HDL = hdl,
                LDL = ldl,
                Triglycerides = triglycerides
            };

            await Context.CholesterolMeasurements.AddAsync(measurement);
            await Context.SaveChangesAsync();

            return measurement;
        }

        /// <summary>
        /// Update the properties of the specified measurement
        /// </summary>
        /// <param name="id"></param>
        /// <param name="personId"></param>
        /// <param name="date"></param>
        /// <param name="total"></param>
        /// <param name="hdl"></param>
        /// <param name="ldl"></param>
        /// <param name="triglycerides"></param>
        /// <returns></returns>
        public async Task<CholesterolMeasurement> UpdateAsync(int id, int personId, DateTime date, decimal total, decimal hdl, decimal ldl, decimal triglycerides)
        {
            Factory.Logger.LogMessage(Severity.Info, $"Updating cholesterol measurementwith ID {id}: Person ID {personId}, {date.ToShortDateString()}, Total {total}, HDL {hdl}, LDL {ldl}, Triclycerides {triglycerides}");

            var measurement = Context.CholesterolMeasurements.FirstOrDefault(x => x.Id == id);
            if (measurement != null)
            {
                CheckPersonExists(personId);

                // Save the changes
                measurement.PersonId = personId;
                measurement.Date = date;
                measurement.Total = total;
                measurement.HDL = hdl;
                measurement.LDL = ldl;
                measurement.Triglycerides = triglycerides;
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
            Factory.Logger.LogMessage(Severity.Info, $"Deleting cholesterol measurementwith ID {id}");

            var measurement = Context.CholesterolMeasurements.FirstOrDefault(x => x.Id == id);
            if (measurement != null)
            {
                Factory.Context.Remove(measurement);
                await Factory.Context.SaveChangesAsync();
            }
        }
    }
}

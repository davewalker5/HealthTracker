using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HealthTracker.Logic.Database
{
    public class BloodOxygenSaturationBandManager : DatabaseManagerBase, IBloodOxygenSaturationBandManager
    {
        internal BloodOxygenSaturationBandManager(IHealthTrackerFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return all bands matching the specified criteria ordered by the order in which they are applied
        /// during assessment
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<List<BloodOxygenSaturationBand>> ListAsync(Expression<Func<BloodOxygenSaturationBand, bool>> predicate)
            => await Context.BloodOxygenSaturationBands
                            .Where(predicate)
                            .ToListAsync();
    }
}

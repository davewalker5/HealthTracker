using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HealthTracker.Logic.Database
{
    public class BMIBandManager : DatabaseManagerBase, IBMIBandManager
    {
        internal BMIBandManager(IHealthTrackerFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return all bands matching the specified criteria ordered by the order in which they are applied
        /// during assessment
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<List<BMIBand>> ListAsync(Expression<Func<BMIBand, bool>> predicate)
            => await Context.BMIBands
                            .Where(predicate)
                            .OrderBy(x => x.Order)
                            .ToListAsync();
    }
}

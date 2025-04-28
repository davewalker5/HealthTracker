using HealthTracker.Entities.Measurements;
using System.Linq.Expressions;

namespace HealthTracker.Entities.Interfaces
{
    public interface IBloodOxygenSaturationBandManager
    {
        Task<List<BloodOxygenSaturationBand>> ListAsync(Expression<Func<BloodOxygenSaturationBand, bool>> predicate);
    }
}

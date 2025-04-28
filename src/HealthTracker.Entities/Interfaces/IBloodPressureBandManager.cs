using HealthTracker.Entities.Measurements;
using System.Linq.Expressions;

namespace HealthTracker.Entities.Interfaces
{
    public interface IBloodPressureBandManager
    {
        Task<List<BloodPressureBand>> ListAsync(Expression<Func<BloodPressureBand, bool>> predicate);
    }
}

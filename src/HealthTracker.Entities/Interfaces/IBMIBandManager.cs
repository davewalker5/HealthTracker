using HealthTracker.Entities.Measurements;
using System.Linq.Expressions;

namespace HealthTracker.Entities.Interfaces
{
    public interface IBMIBandManager
    {
        Task<List<BMIBand>> ListAsync(Expression<Func<BMIBand, bool>> predicate);
    }
}

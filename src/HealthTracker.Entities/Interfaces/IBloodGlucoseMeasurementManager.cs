using System.Linq.Expressions;
using HealthTracker.Entities.Measurements;

namespace HealthTracker.Entities.Interfaces
{
    public interface IBloodGlucoseMeasurementManager
    {
        Task<List<BloodGlucoseMeasurement>> ListAsync(Expression<Func<BloodGlucoseMeasurement, bool>> predicate, int pageNumber, int pageSize);
        Task<BloodGlucoseMeasurement> AddAsync(int personId, DateTime date, decimal level);
        Task<BloodGlucoseMeasurement> UpdateAsync(int id, int personId, DateTime date, decimal level);
        Task DeleteAsync(int id);
    }
}

using HealthTracker.Entities.Measurements;
using System.Linq.Expressions;

namespace HealthTracker.Entities.Interfaces
{
    public interface IWeightMeasurementManager
    {
        Task<WeightMeasurement> AddAsync(int personId, DateTime date, decimal weight);
        Task DeleteAsync(int id);
        Task<List<WeightMeasurement>> ListAsync(Expression<Func<WeightMeasurement, bool>> predicate);
        Task<WeightMeasurement> UpdateAsync(int id, int personId, DateTime date, decimal weight);
    }
}
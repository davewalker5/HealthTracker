using HealthTracker.Entities.Measurements;
using System.Linq.Expressions;

namespace HealthTracker.Entities.Interfaces
{
    public interface IBloodPressureMeasurementManager
    {
        Task<BloodPressureMeasurement> AddAsync(int personId, DateTime date, int systolic, int diastolic);
        Task DeleteAsync(int id);
        Task<List<BloodPressureMeasurement>> ListAsync(Expression<Func<BloodPressureMeasurement, bool>> predicate);
        Task<BloodPressureMeasurement> UpdateAsync(int id, int personId, DateTime date, int systolic, int diastolic);
    }
}

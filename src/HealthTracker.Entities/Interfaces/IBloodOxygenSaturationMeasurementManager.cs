using System.Linq.Expressions;
using HealthTracker.Entities.Measurements;

namespace HealthTracker.Entities.Interfaces
{
    public interface IBloodOxygenSaturationMeasurementManager
    {
        Task<List<BloodOxygenSaturationMeasurement>> ListAsync(Expression<Func<BloodOxygenSaturationMeasurement, bool>> predicate, int pageNumber, int pageSize);
        Task<BloodOxygenSaturationMeasurement> AddAsync(int personId, DateTime date, decimal spo2);
        Task<BloodOxygenSaturationMeasurement> UpdateAsync(int id, int personId, DateTime date, decimal spo2);
        Task DeleteAsync(int id);
    }
}

using HealthTracker.Entities.Measurements;
using System.Linq.Expressions;

namespace HealthTracker.Entities.Interfaces
{
    public interface ICholesterolMeasurementManager
    {
        Task<CholesterolMeasurement> AddAsync(int personId, DateTime date, decimal total, decimal hdl, decimal ldl, decimal triglycerides);
        Task DeleteAsync(int id);
        Task<List<CholesterolMeasurement>> ListAsync(Expression<Func<CholesterolMeasurement, bool>> predicate);
        Task<CholesterolMeasurement> UpdateAsync(int id, int personId, DateTime date, decimal total, decimal hdl, decimal ldl, decimal triglycerides);
    }
}

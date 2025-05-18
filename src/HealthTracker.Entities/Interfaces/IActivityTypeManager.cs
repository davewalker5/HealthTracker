using HealthTracker.Entities.Measurements;
using System.Linq.Expressions;

namespace HealthTracker.Entities.Interfaces
{
    public interface IActivityTypeManager
    {
        Task<ActivityType> AddAsync(string description, bool distanceBased);
        Task DeleteAsync(int id);
        Task<ActivityType> GetAsync(Expression<Func<ActivityType, bool>> predicate);
        Task<List<ActivityType>> ListAsync(Expression<Func<ActivityType, bool>> predicate, int pageNumber, int pageSize);
        Task<ActivityType> UpdateAsync(int id, string description, bool distanceBased);
    }
}

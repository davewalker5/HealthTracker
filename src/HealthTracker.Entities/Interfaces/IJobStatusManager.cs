using HealthTracker.Entities.Logging;
using System.Linq.Expressions;

namespace HealthTracker.Entities.Interfaces
{
    public interface IJobStatusManager
    {
        Task<JobStatus> AddAsync(string name, string parameters);
        Task<JobStatus> GetAsync(Expression<Func<JobStatus, bool>> predicate);
        Task<List<JobStatus>> ListAsync(Expression<Func<JobStatus, bool>> predicate, int pageNumber, int pageSize);
        Task<JobStatus> UpdateAsync(long id, string error);
    }
}
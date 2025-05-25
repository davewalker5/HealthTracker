using HealthTracker.Entities.Logging;

namespace HealthTracker.Client.Interfaces
{
    public interface IJobStatusClient
    {
        Task<List<JobStatus>> ListAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize);
    }
}
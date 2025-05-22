using HealthTracker.Entities.Measurements;

namespace HealthTracker.Client.Interfaces
{
    public interface IActivityTypeClient
    {
        Task<ActivityType> AddAsync(string description, bool distanceBased);
        Task DeleteAsync(int id);
        Task<List<ActivityType>> ListAsync(int pageNumber, int pageSize);
        Task<ActivityType> UpdateAsync(int id, string description, bool distanceBased);
    }
}
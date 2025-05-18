using HealthTracker.Entities.Measurements;

namespace HealthTracker.Client.Interfaces
{
    public interface IActivityTypeClient
    {
        Task<ActivityType> AddActivityTypeAsync(string description, bool distanceBased);
        Task DeleteActivityTypeAsync(int id);
        Task<List<ActivityType>> ListActivityTypesAsync(int pageNumber, int pageSize);
        Task<ActivityType> UpdateActivityTypeAsync(int id, string description, bool distanceBased);
    }
}
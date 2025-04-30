using HealthTracker.Entities.Measurements;

namespace HealthTracker.Client.Interfaces
{
    public interface IActivityTypeClient
    {
        Task<ActivityType> AddActivityTypeAsync(string description);
        Task DeleteActivityTypeAsync(int id);
        Task<List<ActivityType>> ListActivityTypesAsync();
        Task<ActivityType> UpdateActivityTypeAsync(int id, string description);
    }
}
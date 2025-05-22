using HealthTracker.Entities.Medications;

namespace HealthTracker.Client.Interfaces
{
    public interface IMedicationClient
    {
        Task<Medication> AddAsync(string name);
        Task<Medication> UpdateAsync(int id, string name);
        Task DeleteAsync(int id);
        Task<List<Medication>> ListAsync(int pageNumber, int pageSize);
    }
}
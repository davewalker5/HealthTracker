using HealthTracker.Entities.Medications;

namespace HealthTracker.Client.Interfaces
{
    public interface IMedicationClient
    {
        Task<Medication> AddMedicationAsync(string name);
        Task<Medication> UpdateMedicationAsync(int id, string name);
        Task DeleteMedicationAsync(int id);
        Task<List<Medication>> ListMedicationsAsync();
    }
}
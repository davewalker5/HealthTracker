using HealthTracker.Entities.Medications;

namespace HealthTracker.Client.Interfaces
{
    public interface IPersonMedicationClient : IPersonBasedEntityRetriever<PersonMedication>
    {
        Task<PersonMedication> AddAsync(int personId, int medicationId, int dose, int stock, DateTime? lastTaken);
        Task<PersonMedication> UpdateAsync(int id, int personId, int medicationId, int dose, int stock, bool active, DateTime? lastTaken);
        Task<PersonMedication> ActivateAsync(int id);
        Task<PersonMedication> DeactivateAsync(int id);
        Task DeleteAsync(int id);
    }
}
using HealthTracker.Entities.Medications;

namespace HealthTracker.Client.Interfaces
{
    public interface IPersonMedicationClient
    {
        Task<PersonMedication> AddPersonMedicationAsync(int personId, int medicationId, int dose, int stock, DateTime? lastTaken);
        Task<PersonMedication> UpdatePersonMedicationAsync(int id, int personId, int medicationId, int dose, int stock, bool active, DateTime? lastTaken);
        Task<PersonMedication> ActivatePersonMedicationAsync(int id);
        Task<PersonMedication> DeactivatePersonMedicationAsync(int id);
        Task DeletePersonMedicationAsync(int id);
        Task<List<PersonMedication>> ListPersonMedicationsAsync(int personId);
    }
}
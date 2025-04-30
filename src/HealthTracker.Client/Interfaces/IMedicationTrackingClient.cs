using HealthTracker.Entities.Medications;

namespace HealthTracker.Client.Interfaces
{
    public interface IMedicationTrackingClient
    {
        Task<PersonMedication> AddMedicationStockAsync(int personId, int medicationId, int tablets);
        Task<PersonMedication> SetMedicationStockAsync(int personId, int medicationId, int tablets);
        Task<PersonMedication> SetMedicationDoseAsync(int personId, int medicationId, int tablets);
        Task<PersonMedication> TakeDoseAsync(int personId, int medicationId);
        Task TakeAllDosesAsync(int personId);
        Task<PersonMedication> UntakeDoseAsync(int personId, int medicationId);
        Task UntakeAllDosesAsync(int personId);
        Task<PersonMedication> FastForwardAsync(int personId, int medicationId);
        Task FastForwardAllAsync(int personId);
        Task<PersonMedication> SkipDoseAsync(int personId, int medicationId);
        Task SkipAllDosesAsync(int personId);
    }
}
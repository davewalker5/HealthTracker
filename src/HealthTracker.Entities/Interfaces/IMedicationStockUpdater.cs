using HealthTracker.Entities.Medications;

namespace HealthTracker.Entities.Interfaces
{
    public interface IMedicationStockUpdater
    {
        Task<PersonMedication> AddStockAsync(int id, int tablets);
        Task<PersonMedication> SetStockAsync(int id, int tablets);
        Task<PersonMedication> DecrementAsync(int id, int doses);
        Task DecrementAllAsync(int personId, int doses);
        Task<PersonMedication> IncrementAsync(int id, int doses);
        Task IncrementAllAsync(int personId, int doses);
        Task<PersonMedication> FastForwardAsync(int id);
        Task FastForwardAllAsync(int personId);
        Task<PersonMedication> SkipAsync(int id);
        Task SkipAllAsync(int personId);
    }
}
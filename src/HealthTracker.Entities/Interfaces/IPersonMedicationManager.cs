using HealthTracker.Entities.Medications;
using System.Linq.Expressions;

namespace HealthTracker.Entities.Interfaces
{
    public interface IPersonMedicationManager
    {
        Task<List<PersonMedication>> ListAsync(Expression<Func<PersonMedication, bool>> predicate);
        Task<PersonMedication> AddAsync(int personId, int medicationId, int dose, int stock, DateTime? taken);
        Task<PersonMedication> UpdateAsync(int id, int personId, int medicationId, int dose, int stock, DateTime? taken, bool active);
        Task<PersonMedication> ActivateAsync(int id);
        Task<PersonMedication> DeactivateAsync(int id);
        Task DeleteAsync(int id);
        Task<PersonMedication> SetDoseAsync(int id, int tablets);
        Task<PersonMedication> SetStockAsync(int id, int tablets);
        Task<PersonMedication> SetStockAsync(int id, int tablets, DateTime? lastTaken);
        Task<PersonMedication> AddStockAsync(int id, int tablets);
    }
}
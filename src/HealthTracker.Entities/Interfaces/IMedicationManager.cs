using System.Linq.Expressions;
using HealthTracker.Entities.Medications;

namespace HealthTracker.Entities.Interfaces
{
    public interface IMedicationManager
    {
        Task<Medication> GetAsync(Expression<Func<Medication, bool>> predicate);
        Task<List<Medication>> ListAsync(Expression<Func<Medication, bool>> predicate);
        Task<Medication> AddAsync(string name);
        Task<Medication> UpdateAsync(int id, string name);
        Task DeleteAsync(int id);
    }
}
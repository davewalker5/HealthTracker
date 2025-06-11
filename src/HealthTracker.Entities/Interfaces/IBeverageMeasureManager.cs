using HealthTracker.Entities.Food;
using System.Linq.Expressions;

namespace HealthTracker.Entities.Interfaces
{
    public interface IBeverageMeasureManager
    {
        Task<BeverageMeasure> AddAsync(string name, decimal volume);
        Task DeleteAsync(int id);
        Task<BeverageMeasure> GetAsync(Expression<Func<BeverageMeasure, bool>> predicate);
        Task<List<BeverageMeasure>> ListAsync(Expression<Func<BeverageMeasure, bool>> predicate, int pageNumber, int pageSize);
        Task<BeverageMeasure> UpdateAsync(int id, string name, decimal volume);
    }
}

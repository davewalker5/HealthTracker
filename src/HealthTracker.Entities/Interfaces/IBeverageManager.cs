using HealthTracker.Entities.Food;
using System.Linq.Expressions;

namespace HealthTracker.Entities.Interfaces
{
    public interface IBeverageManager
    {
        Task<Beverage> AddAsync(string name, decimal typicalABV, bool isHydrating, bool isAlcohol);
        Task DeleteAsync(int id);
        Task<Beverage> GetAsync(Expression<Func<Beverage, bool>> predicate);
        Task<List<Beverage>> ListAsync(Expression<Func<Beverage, bool>> predicate, int pageNumber, int pageSize);
        Task<Beverage> UpdateAsync(int id, string name, decimal typicalABV, bool isHydrating, bool isAlcohol);
        void CheckBeverageExists(int beverageId);
    }
}

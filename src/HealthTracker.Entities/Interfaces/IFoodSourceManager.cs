using HealthTracker.Entities.Food;
using System.Linq.Expressions;

namespace HealthTracker.Entities.Interfaces
{
    public interface IFoodSourceManager
    {
        Task<FoodSource> AddAsync(string name);
        Task DeleteAsync(int id);
        Task<FoodSource> GetAsync(Expression<Func<FoodSource, bool>> predicate);
        Task<List<FoodSource>> ListAsync(Expression<Func<FoodSource, bool>> predicate, int pageNumber, int pageSize);
        Task<FoodSource> UpdateAsync(int id, string name);
    }
}

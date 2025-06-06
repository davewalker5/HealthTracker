using HealthTracker.Entities.Food;
using System.Linq.Expressions;

namespace HealthTracker.Entities.Interfaces
{
    public interface IFoodCategoryManager
    {
        Task<FoodCategory> AddAsync(string name);
        Task DeleteAsync(int id);
        Task<FoodCategory> GetAsync(Expression<Func<FoodCategory, bool>> predicate);
        Task<List<FoodCategory>> ListAsync(Expression<Func<FoodCategory, bool>> predicate, int pageNumber, int pageSize);
        Task<FoodCategory> UpdateAsync(int id, string name);
    }
}

using HealthTracker.Entities.Food;
using System.Linq.Expressions;

namespace HealthTracker.Entities.Interfaces
{
    public interface IFoodItemManager
    {
        Task<List<FoodItem>> ListAsync(Expression<Func<FoodItem, bool>> predicate, int pageNumber, int pageSize);
        Task<FoodItem> AddAsync(string name, decimal portion, int categoryId, int? nutritionalValueId);
        Task<FoodItem> UpdateAsync(int id, string name, decimal portion, int categoryId, int? nutritionalValueId);
        Task DeleteAsync(int id);
        void CheckFoodItemExists(int id);
    }
}

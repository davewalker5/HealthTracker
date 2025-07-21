using HealthTracker.Entities.Food;
using System.Linq.Expressions;

namespace HealthTracker.Entities.Interfaces
{
    public interface IMealFoodItemManager
    {
        Task<MealFoodItem> GetAsync(Expression<Func<MealFoodItem, bool>> predicate);
        Task<List<MealFoodItem>> ListAsync(Expression<Func<MealFoodItem, bool>> predicate);
        Task<MealFoodItem> AddAsync(int mealId, int foodItemId);
        Task<MealFoodItem> UpdateAsync(int id, int mealId, int foodItemId);
        Task DeleteAsync(int id);
    }
}
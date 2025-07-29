using HealthTracker.Entities.Food;
using HealthTracker.Enumerations.Enumerations;
using System.Linq.Expressions;

namespace HealthTracker.Entities.Interfaces
{
    public interface IPlannedMealManager
    {
        Task<PlannedMeal> GetAsync(Expression<Func<PlannedMeal, bool>> predicate);
        Task<List<PlannedMeal>> ListAsync(Expression<Func<PlannedMeal, bool>> predicate, int pageNumber, int pageSize);
        Task<PlannedMeal> AddAsync(int personId, MealType mealType, DateTime date, int mealId);
        Task<PlannedMeal> UpdateAsync(int id, int personId, MealType mealType, DateTime date, int mealId);
        Task DeleteAsync(int id);
        Task PurgeAsync(int personId, DateTime? cutoff);
        Task<List<ShoppingListItem>> GetShoppingList(int personId, DateTime from, DateTime to);
    }
}

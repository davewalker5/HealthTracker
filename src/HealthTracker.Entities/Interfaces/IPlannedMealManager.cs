using HealthTracker.Entities.Food;
using HealthTracker.Enumerations.Enumerations;
using System.Linq.Expressions;

namespace HealthTracker.Entities.Interfaces
{
    public interface IPlannedMealManager
    {
        Task<PlannedMeal> GetAsync(Expression<Func<PlannedMeal, bool>> predicate);
        Task<List<PlannedMeal>> ListAsync(Expression<Func<PlannedMeal, bool>> predicate, int pageNumber, int pageSize);
        Task<PlannedMeal> AddAsync(MealType mealType, DateTime date, int mealId);
        Task<PlannedMeal> UpdateAsync(int id, MealType mealType, DateTime date, int mealId);
        Task DeleteAsync(int id);
        Task Purge(DateTime? cutoff);
    }
}

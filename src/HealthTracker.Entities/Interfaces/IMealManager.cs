using HealthTracker.Entities.Food;
using System.Linq.Expressions;

namespace HealthTracker.Entities.Interfaces
{
    public interface IMealManager
    {
        Task<List<Meal>> ListAsync(Expression<Func<Meal, bool>> predicate, int pageNumber, int pageSize);
        Task<List<Meal>> SearchAsync(MealSearchCriteria criteria, int pageNumber, int pageSize);
        Task<Meal> AddAsync(string name, int portions, int foodSourceId, string reference, int? nutritionalValueId);
        Task<Meal> UpdateAsync(int id, string name, int portions, int foodSourceId, string reference, int? nutritionalValueId);
        Task DeleteAsync(int id);
        void CheckMealExists(int mealId);
        Task UpdateNutritionalValues(int id);
        Task UpdateAllNutritionalValues();
    }
}

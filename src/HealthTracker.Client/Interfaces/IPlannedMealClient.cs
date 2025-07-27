using HealthTracker.Entities.Food;
using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.Client.Interfaces
{
    public interface IPlannedMealClient : IEntityRetriever<PlannedMeal>, IDataImporterExporter
    {
        Task<PlannedMeal> AddAsync(MealType mealType, DateTime date, int mealId);
        Task<PlannedMeal> UpdateAsync(int id, MealType mealType, DateTime date, int mealId);
        Task DeleteAsync(int id);
        Task PurgeAsync(DateTime? cutoff);
        Task<List<PlannedMeal>> ListAsync(int pageNumber, int pageSize);
    }
}
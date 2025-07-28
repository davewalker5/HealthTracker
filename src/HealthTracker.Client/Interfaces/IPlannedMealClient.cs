using HealthTracker.Entities.Food;
using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.Client.Interfaces
{
    public interface IPlannedMealClient : IDateBasedEntityRetriever<PlannedMeal>, IMeasurementImporterExporter
    {
        Task<PlannedMeal> AddAsync(int personId, MealType mealType, DateTime date, int mealId);
        Task<PlannedMeal> UpdateAsync(int id, int personId, MealType mealType, DateTime date, int mealId);
        Task DeleteAsync(int id);
        Task PurgeAsync(int personId, DateTime? cutoff);
    }
}
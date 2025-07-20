using HealthTracker.Entities.Food;

namespace HealthTracker.Client.Interfaces
{
    public interface IMealConsumptionMeasurementHelper
    {
        Task<MealConsumptionMeasurement> AddAsync(int personId, int mealId, DateTime date, decimal quantity);
        Task<MealConsumptionMeasurement> UpdateAsync(int id, int personId, int mealId, DateTime date, decimal quantity);
    }
}
using HealthTracker.Entities.Food;

namespace HealthTracker.Entities.Interfaces
{
    public interface IMealConsumptionCalculator
    {
        Task<List<MealConsumptionDailySummary>> DailyTotalConsumptionAsync(int personId, DateTime from, DateTime to);
    }
}

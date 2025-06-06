using HealthTracker.Entities.Food;

namespace HealthTracker.Entities.Interfaces
{
    public interface IBeverageConsumptionCalculator
    {
        Task<List<BeverageConsumptionMeasurement>> DailyTotalHydratingAsync(int personId, DateTime from, DateTime to);
        Task<List<BeverageConsumptionMeasurement>> DailyTotalAlcoholAsync(int personId, DateTime from, DateTime to);
        Task<BeverageConsumptionSummary> TotalHydratingAsync(int personId, DateTime from, DateTime to);
        Task<BeverageConsumptionSummary> TotalAlcoholAsync(int personId, DateTime from, DateTime to);
    }
}

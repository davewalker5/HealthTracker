using HealthTracker.Entities.Measurements;

namespace HealthTracker.Entities.Interfaces
{
    public interface IBeverageConsumptionCalculator
    {
        Task<List<BeverageConsumptionSummary>> DailyTotalHydratingAsync(int personId, DateTime from, DateTime to);
        Task<List<BeverageConsumptionSummary>> DailyTotalAlcoholAsync(int personId, DateTime from, DateTime to);
        Task<BeverageConsumptionSummary> TotalHydratingAsync(int personId, DateTime from, DateTime to);
        Task<BeverageConsumptionSummary> TotalAlcoholAsync(int personId, DateTime from, DateTime to);
    }
}

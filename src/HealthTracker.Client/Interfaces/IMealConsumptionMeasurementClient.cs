using HealthTracker.Entities.Food;

namespace HealthTracker.Client.Interfaces
{
    public interface IMealConsumptionMeasurementClient : IDateBasedEntityRetriever<MealConsumptionMeasurement>, IMeasurementImporterExporter
    {
        Task<MealConsumptionMeasurement> AddAsync(
            int personId,
            int mealId,
            DateTime? date,
            decimal quantity);

        Task DeleteAsync(int id);

        Task<MealConsumptionMeasurement> UpdateAsync(
            int id,
            int personId,
            int mealId,
            DateTime? date,
            decimal quantity);

        Task UpdateAllNutritionalValues();
        Task<List<MealConsumptionDailySummary>> CalculateDailyTotalConsumption(int personId, DateTime? from, DateTime? to);
    }
}
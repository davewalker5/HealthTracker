using HealthTracker.Entities.Food;

namespace HealthTracker.Client.Interfaces
{
    public interface IMealConsumptionMeasurementClient : IDateBasedEntityRetriever<MealConsumptionMeasurement>, IMeasurementImporterExporter
    {
        Task<MealConsumptionMeasurement> AddAsync(
            int personId,
            int mealId,
            int? nutritionalValueId,
            DateTime? date,
            int quantity);

        Task DeleteAsync(int id);

        Task<MealConsumptionMeasurement> UpdateAsync(
            int id,
            int personId,
            int mealId,
            int? nutritionalValueId,
            DateTime? date,
            int quantity);
    }
}
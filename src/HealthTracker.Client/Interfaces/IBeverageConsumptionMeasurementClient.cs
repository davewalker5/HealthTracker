using HealthTracker.Entities.Measurements;
using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.Client.Interfaces
{
    public interface IBeverageConsumptionMeasurementClient : IDateBasedEntityRetriever<BeverageConsumptionMeasurement>, IImporterExporter
    {
        Task<BeverageConsumptionMeasurement> AddAsync(
            int personId,
            int beverageId,
            DateTime? date,
            BeverageMeasure measure,
            int quantity,
            decimal abv);

        Task DeleteAsync(int id);

        Task<BeverageConsumptionMeasurement> UpdateAsync(
            int id,
            int personId,
            int beverageId,
            DateTime? date,
            BeverageMeasure measure,
            int quantity,
            decimal abv);

        Task<BeverageConsumptionSummary> CalculateTotalHydratingAsync(int personId, int days);
        Task<BeverageConsumptionSummary> CalculateTotalHydratingAsync(int personId, DateTime from, DateTime to);
        Task<List<BeverageConsumptionSummary>> CalculateDailyTotalHydratingAsync(int personId, DateTime from, DateTime to);
        Task<BeverageConsumptionSummary> CalculateTotalAlcoholicAsync(int personId, int days);
        Task<BeverageConsumptionSummary> CalculateTotalAlcoholicAsync(int personId, DateTime from, DateTime to);
        Task<List<BeverageConsumptionSummary>> CalculateDailyTotalAlcoholicAsync(int personId, DateTime from, DateTime to);
    }
}
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
    }
}
using HealthTracker.Client.ApiClient;
using HealthTracker.Entities.Measurements;
using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.Client.Interfaces
{
    public interface IAlcoholConsumptionMeasurementClient : IDateBasedEntityRetriever<AlcoholConsumptionMeasurement>, IImporterExporter
    {
        Task<AlcoholConsumptionMeasurement> AddAsync(
            int personId,
            int beverageId,
            DateTime? date,
            AlcoholMeasure measure,
            int quantity,
            decimal abv);

        Task DeleteAsync(int id);

        Task<AlcoholConsumptionMeasurement> UpdateAsync(
            int id,
            int personId,
            int beverageId,
            DateTime? date,
            AlcoholMeasure measure,
            int quantity,
            decimal abv);
    }
}
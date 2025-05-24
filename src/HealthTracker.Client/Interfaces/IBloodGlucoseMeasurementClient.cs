using HealthTracker.Entities.Measurements;

namespace HealthTracker.Client.Interfaces
{
    public interface IBloodGlucoseMeasurementClient : IDateBasedEntityRetriever<BloodGlucoseMeasurement>, IImporterExporter
    {
        Task<BloodGlucoseMeasurement> AddAsync(int personId, DateTime? date, decimal level);
        Task<BloodGlucoseMeasurement> UpdateAsync(int id, int personId, DateTime? date, decimal level);
        Task DeleteAsync(int id);
    }
}
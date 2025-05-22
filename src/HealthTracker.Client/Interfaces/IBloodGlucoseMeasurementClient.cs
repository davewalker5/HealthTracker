using HealthTracker.Entities.Measurements;

namespace HealthTracker.Client.Interfaces
{
    public interface IBloodGlucoseMeasurementClient : IDateBasedEntityRetriever<BloodGlucoseMeasurement>
    {
        Task<BloodGlucoseMeasurement> AddAsync(int personId, DateTime? date, decimal level);
        Task<BloodGlucoseMeasurement> UpdateAsync(int id, int personId, DateTime? date, decimal level);
        Task ImportAsync(string filePath);
        Task ExportAsync(int personId, DateTime? from, DateTime? to, string fileName);
        Task DeleteAsync(int id);
    }
}
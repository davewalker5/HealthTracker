using HealthTracker.Entities.Measurements;

namespace HealthTracker.Client.Interfaces
{
    public interface IBloodGlucoseMeasurementClient : IEntityRetriever<BloodGlucoseMeasurement>
    {
        Task<BloodGlucoseMeasurement> AddBloodGlucoseMeasurementAsync(int personId, DateTime? date, decimal level);
        Task<BloodGlucoseMeasurement> UpdateBloodGlucoseMeasurementAsync(int id, int personId, DateTime? date, decimal level);
        Task ImportBloodGlucoseMeasurementsAsync(string filePath);
        Task ExportBloodGlucoseMeasurementsAsync(int personId, DateTime? from, DateTime? to, string fileName);
        Task DeleteBloodGlucoseMeasurementAsync(int id);
        Task<List<BloodGlucoseMeasurement>> ListBloodGlucoseMeasurementsAsync(int personId, DateTime? from, DateTime? to, int pageNumber, int pageSize);
    }
}
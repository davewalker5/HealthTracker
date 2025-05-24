using HealthTracker.Entities.Measurements;

namespace HealthTracker.Client.Interfaces
{
    public interface IBloodOxygenSaturationMeasurementClient : IDateBasedEntityRetriever<BloodOxygenSaturationMeasurement>, IImporterExporter
    {
        Task<BloodOxygenSaturationMeasurement> AddAsync(int personId, DateTime? date, decimal percentage);
        Task<BloodOxygenSaturationMeasurement> UpdateAsync(int id, int personId, DateTime? date, decimal percentage);
        Task DeleteAsync(int id);
        Task<BloodOxygenSaturationMeasurement> CalculateAverageAsync(int personId, DateTime from, DateTime to);
        Task<BloodOxygenSaturationMeasurement> CalculateAverageAsync(int personId, int days);
        Task<List<BloodOxygenSaturationMeasurement>> CalculateDailyAverageAsync(int personId, DateTime from, DateTime to);
        Task ExportDailyAverageAsync(int personId, DateTime from, DateTime to, string fileName);
    }
}
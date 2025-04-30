using HealthTracker.Entities.Measurements;

namespace HealthTracker.Client.Interfaces
{
    public interface IBloodOxygenSaturationMeasurementClient
    {
        Task<BloodOxygenSaturationMeasurement> AddBloodOxygenSaturationMeasurementAsync(int personId, DateTime? date, decimal percentage);
        Task<BloodOxygenSaturationMeasurement> UpdateBloodOxygenSaturationMeasurementAsync(int id, int personId, DateTime? date, decimal percentage);
        Task ImportBloodOxygenSaturationMeasurementsAsync(string filePath);
        Task ExportBloodOxygenSaturationMeasurementsAsync(int personId, DateTime? from, DateTime? to, string fileName);
        Task DeleteBloodOxygenSaturationMeasurementAsync(int id);
        Task<List<BloodOxygenSaturationMeasurement>> ListBloodOxygenSaturationMeasurementsAsync(int personId, DateTime? from, DateTime? to);
        Task<BloodOxygenSaturationMeasurement> CalculateAverageBloodOxygenSaturationAsync(int personId, DateTime from, DateTime to);
        Task<BloodOxygenSaturationMeasurement> CalculateAverageBloodOxygenSaturationAsync(int personId, int days);
        Task<List<BloodOxygenSaturationMeasurement>> CalculateDailyAverageBloodOxygenSaturationAsync(int personId, DateTime from, DateTime to);
        Task ExportDailyAverageBloodOxygenSaturationAsync(int personId, DateTime from, DateTime to, string fileName);
    }
}
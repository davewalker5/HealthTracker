using HealthTracker.Entities.Measurements;

namespace HealthTracker.Client.Interfaces
{
    public interface IBloodPressureMeasurementClient
    {
        Task<BloodPressureMeasurement> AddBloodPressureMeasurementAsync(int personId, DateTime? date, int systolic, int diastolic);
        Task<BloodPressureMeasurement> CalculateAverageBloodPressureAsync(int personId, DateTime from, DateTime to);
        Task<BloodPressureMeasurement> CalculateAverageBloodPressureAsync(int personId, int days);
        Task<List<BloodPressureMeasurement>> CalculateDailyAverageBloodPressureAsync(int personId, DateTime from, DateTime to);
        Task DeleteBloodPressureMeasurementAsync(int id);
        Task ExportBloodPressureMeasurementsAsync(int personId, DateTime? from, DateTime? to, string fileName);
        Task ExportDailyAverageBloodPressureAsync(int personId, DateTime from, DateTime to, string fileName);
        Task ImportBloodPressureMeasurementsAsync(string filePath);
        Task ImportOmronBloodPressureMeasurementsAsync(string filePath, int personId);
        Task<List<BloodPressureMeasurement>> ListBloodPressureMeasurementsAsync(int personId, DateTime? from, DateTime? to);
        Task<BloodPressureMeasurement> UpdateBloodPressureMeasurementAsync(int id, int personId, DateTime? date, int systolic, int diastolic);
    }
}
using HealthTracker.Entities.Measurements;

namespace HealthTracker.Client.Interfaces
{
    public interface IBloodPressureMeasurementClient : IDateBasedEntityRetriever<BloodPressureMeasurement>, IDataExporter
    {
        Task<BloodPressureMeasurement> AddAsync(int personId, DateTime? date, int systolic, int diastolic);
        Task<BloodPressureMeasurement> CalculateAverageAsync(int personId, DateTime from, DateTime to);
        Task<BloodPressureMeasurement> CalculateAverageAsync(int personId, int days);
        Task<List<BloodPressureMeasurement>> CalculateDailyAverageAsync(int personId, DateTime from, DateTime to);
        Task DeleteAsync(int id);
        Task ExportDailyAverageAsync(int personId, DateTime from, DateTime to, string fileName);
        Task ImportAsync(string filePath);
        Task ImportOmronAsync(string filePath, int personId);
        Task<BloodPressureMeasurement> UpdateAsync(int id, int personId, DateTime? date, int systolic, int diastolic);
    }
}
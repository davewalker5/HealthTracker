using HealthTracker.Entities.Measurements;

namespace HealthTracker.Client.Interfaces
{
    public interface IWeightMeasurementClient : IDateBasedEntityRetriever<WeightMeasurement>, IDataExporter
    {
        Task<WeightMeasurement> AddAsync(int personId, DateTime? date, decimal weight);
        Task<WeightMeasurement> CalculateAverageAsync(int personId, DateTime from, DateTime to);
        Task<WeightMeasurement> CalculateAverageAsync(int personId, int days);
        Task DeleteAsync(int id);
        Task ImportAsync(string filePath);
        Task<WeightMeasurement> UpdateAsync(int id, int personId, DateTime? date, decimal weight);
    }
}
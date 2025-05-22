using HealthTracker.Entities.Measurements;

namespace HealthTracker.Client.Interfaces
{
    public interface IWeightMeasurementClient : IDateBasedEntityRetriever<WeightMeasurement>
    {
        Task<WeightMeasurement> AddAsync(int personId, DateTime? date, decimal weight);
        Task<WeightMeasurement> CalculateAverageAsync(int personId, DateTime from, DateTime to);
        Task<WeightMeasurement> CalculateAverageAsync(int personId, int days);
        Task DeleteAsync(int id);
        Task ExportAsync(int personId, DateTime? from, DateTime? to, string fileName);
        Task ImportAsync(string filePath);
        Task<WeightMeasurement> UpdateAsync(int id, int personId, DateTime? date, decimal weight);
    }
}
using HealthTracker.Entities.Measurements;

namespace HealthTracker.Client.Interfaces
{
    public interface IWeightMeasurementClient : IEntityRetriever<WeightMeasurement>
    {
        Task<WeightMeasurement> AddWeightMeasurementAsync(int personId, DateTime? date, decimal weight);
        Task<WeightMeasurement> CalculateAverageWeightAsync(int personId, DateTime from, DateTime to);
        Task<WeightMeasurement> CalculateAverageWeightAsync(int personId, int days);
        Task DeleteWeightMeasurementAsync(int id);
        Task ExportWeightMeasurementsAsync(int personId, DateTime? from, DateTime? to, string fileName);
        Task ImportWeightMeasurementsAsync(string filePath);
        Task<List<WeightMeasurement>> ListWeightMeasurementsAsync(int personId, DateTime? from, DateTime? to, int pageNumber, int pageSize);
        Task<WeightMeasurement> UpdateWeightMeasurementAsync(int id, int personId, DateTime? date, decimal weight);
    }
}
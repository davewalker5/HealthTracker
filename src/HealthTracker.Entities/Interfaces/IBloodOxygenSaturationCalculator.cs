using HealthTracker.Entities.Measurements;

namespace HealthTracker.Entities.Interfaces
{
    public interface IBloodOxygenSaturationCalculator
    {
        Task<BloodOxygenSaturationMeasurement> AverageAsync(int personId, DateTime from, DateTime to);
        Task<List<BloodOxygenSaturationMeasurement>> DailyAverageAsync(int personId, DateTime from, DateTime to);
    }
}

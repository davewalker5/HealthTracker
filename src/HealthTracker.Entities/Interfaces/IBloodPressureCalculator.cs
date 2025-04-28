using HealthTracker.Entities.Measurements;

namespace HealthTracker.Entities.Interfaces
{
    public interface IBloodPressureCalculator
    {
        Task<BloodPressureMeasurement> AverageAsync(int personId, DateTime from, DateTime to);
        Task<List<BloodPressureMeasurement>> DailyAverageAsync(int personId, DateTime from, DateTime to);
    }
}

using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Measurements;

namespace HealthTracker.Entities.Interfaces
{
    public interface IWeightCalculator
    {
        Task<WeightMeasurement> AverageAsync(int personId, DateTime from, DateTime to);
        Task CalculateRelatedProperties(IEnumerable<WeightMeasurement> measurements);
    }
}
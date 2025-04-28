using HealthTracker.Entities.Measurements;

namespace HealthTracker.Entities.Interfaces
{
    public interface IBloodOxygenSaturationAssessor
    {
        Task Assess(IEnumerable<BloodOxygenSaturationMeasurement> measurements);
    }
}

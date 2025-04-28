using HealthTracker.Entities.Measurements;

namespace HealthTracker.Entities.Interfaces
{
    public interface IBloodPressureAssessor
    {
        Task Assess(IEnumerable<BloodPressureMeasurement> measurements);
    }
}

using HealthTracker.Entities.Measurements;

namespace HealthTracker.Client.Interfaces
{
    public interface IReferenceDataClient
    {
        Task<List<BloodPressureBand>> ListBloodPressureAssessmentBandsAsync();
        Task<List<BloodOxygenSaturationBand>> ListBloodOxygenSaturationAssessmentBandsAsync();
        Task<List<BMIBand>> ListBMIAssessmentBandsAsync();
    }
}
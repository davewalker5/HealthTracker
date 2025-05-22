using HealthTracker.Entities.Measurements;

namespace HealthTracker.Client.Interfaces
{
    public interface ICholesterolMeasurementClient
    {
        Task<CholesterolMeasurement> AddAsync(
            int personId,
            DateTime? date,
            decimal total,
            decimal hdl,
            decimal ldl,
            decimal triglycerides);

        Task DeleteAsync(int id);
        Task ExportAsync(int personId, DateTime? from, DateTime? to, string fileName);
        Task ImportAsync(string filePath);
        Task<List<CholesterolMeasurement>> ListAsync(int personId, DateTime? from, DateTime? to);

        Task<CholesterolMeasurement> UpdateAsync(
            int id,
            int personId,
            DateTime? date,
            decimal total,
            decimal hdl,
            decimal ldl,
            decimal triglycerides);
    }
}
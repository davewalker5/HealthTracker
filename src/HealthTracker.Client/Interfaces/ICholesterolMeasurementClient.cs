using HealthTracker.Entities.Measurements;

namespace HealthTracker.Client.Interfaces
{
    public interface ICholesterolMeasurementClient
    {
        Task<CholesterolMeasurement> AddCholesterolMeasurementAsync(
            int personId,
            DateTime? date,
            decimal total,
            decimal hdl,
            decimal ldl,
            decimal triglycerides);

        Task DeleteCholesterolMeasurementAsync(int id);
        Task ExportCholesterolMeasurementsAsync(int personId, DateTime? from, DateTime? to, string fileName);
        Task ImportCholesterolMeasurementsAsync(string filePath);
        Task<List<CholesterolMeasurement>> ListCholesterolMeasurementsAsync(int personId, DateTime? from, DateTime? to);

        Task<CholesterolMeasurement> UpdateCholesterolMeasurementAsync(
            int id,
            int personId,
            DateTime? date,
            decimal total,
            decimal hdl,
            decimal ldl,
            decimal triglycerides);
    }
}
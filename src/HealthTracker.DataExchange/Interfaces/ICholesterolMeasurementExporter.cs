using HealthTracker.DataExchange.Entities;
using HealthTracker.Entities.Measurements;

namespace HealthTracker.DataExchange.Interfaces
{
    public interface ICholesterolMeasurementExporter
    {
        event EventHandler<ExportEventArgs<ExportableCholesterolMeasurement>> RecordExport;
        Task ExportAsync(int personId, DateTime? from, DateTime? to, string file);
        Task ExportAsync(IEnumerable<CholesterolMeasurement> measurements, string file);
    }
}

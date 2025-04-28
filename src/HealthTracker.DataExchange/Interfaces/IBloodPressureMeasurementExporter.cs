using HealthTracker.DataExchange.Entities;
using HealthTracker.Entities.Measurements;

namespace HealthTracker.DataExchange.Interfaces
{
    public interface IBloodPressureMeasurementExporter
    {
        event EventHandler<ExportEventArgs<ExportableBloodPressureMeasurement>> RecordExport;
        Task ExportAsync(int personId, DateTime? from, DateTime? to, string file);
        Task ExportDailyAveragesAsync(int personId, DateTime from, DateTime to, string file);
        Task ExportAsync(IEnumerable<BloodPressureMeasurement> measurements, string file);
    }
}

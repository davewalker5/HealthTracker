using HealthTracker.DataExchange.Entities;
using HealthTracker.Entities.Measurements;

namespace HealthTracker.DataExchange.Interfaces
{
    public interface IBloodOxygenSaturationMeasurementExporter
    {
        event EventHandler<ExportEventArgs<ExportableBloodOxygenSaturationMeasurement>> RecordExport;
        Task ExportAsync(int personId, DateTime? from, DateTime? to, string file);
        Task ExportDailyAveragesAsync(int personId, DateTime from, DateTime to, string file);
        Task ExportAsync(IEnumerable<BloodOxygenSaturationMeasurement> measurements, string file);
    }
}

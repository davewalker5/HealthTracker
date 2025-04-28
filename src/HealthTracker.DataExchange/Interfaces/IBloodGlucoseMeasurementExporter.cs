using HealthTracker.DataExchange.Entities;
using HealthTracker.Entities.Measurements;

namespace HealthTracker.DataExchange.Interfaces
{
    public interface IBloodGlucoseMeasurementExporter
    {
        event EventHandler<ExportEventArgs<ExportableBloodGlucoseMeasurement>> RecordExport;
        Task ExportAsync(int personId, DateTime? from, DateTime? to, string file);
        Task ExportAsync(IEnumerable<BloodGlucoseMeasurement> measurements, string file);
    }
}

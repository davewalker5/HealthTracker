using HealthTracker.DataExchange.Entities;
using HealthTracker.Entities.Measurements;

namespace HealthTracker.DataExchange.Interfaces
{
    public interface IWeightMeasurementExporter
    {
        event EventHandler<ExportEventArgs<ExportableWeightMeasurement>> RecordExport;
        Task ExportAsync(int personId, DateTime? from, DateTime? to, string file);
        Task ExportAsync(IEnumerable<WeightMeasurement> measurements, string file);
    }
}

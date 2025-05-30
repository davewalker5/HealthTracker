using HealthTracker.DataExchange.Entities;
using HealthTracker.Entities.Measurements;

namespace HealthTracker.DataExchange.Interfaces
{
    public interface IBeverageConsumptionMeasurementExporter
    {
        event EventHandler<ExportEventArgs<ExportableBeverageConsumptionMeasurement>> RecordExport;
        Task ExportAsync(int personId, DateTime? from, DateTime? to, string file);
        Task ExportAsync(IEnumerable<BeverageConsumptionMeasurement> measurements, string file);
    }
}

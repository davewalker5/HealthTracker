using HealthTracker.DataExchange.Entities;
using HealthTracker.Entities.Measurements;

namespace HealthTracker.DataExchange.Interfaces
{
    public interface IAlcoholConsumptionMeasurementExporter
    {
        event EventHandler<ExportEventArgs<ExportableAlcoholConsumptionMeasurement>> RecordExport;
        Task ExportAsync(int personId, DateTime? from, DateTime? to, string file);
        Task ExportAsync(IEnumerable<AlcoholConsumptionMeasurement> measurements, string file);
    }
}

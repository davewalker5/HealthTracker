using HealthTracker.DataExchange.Entities;
using HealthTracker.Entities.Food;

namespace HealthTracker.DataExchange.Interfaces
{
    public interface IMealConsumptionMeasurementExporter
    {
        event EventHandler<ExportEventArgs<ExportableMealConsumptionMeasurement>> RecordExport;
        Task ExportAsync(int personId, DateTime? from, DateTime? to, string file);
        Task ExportAsync(IEnumerable<MealConsumptionMeasurement> measurements, string file);
    }
}

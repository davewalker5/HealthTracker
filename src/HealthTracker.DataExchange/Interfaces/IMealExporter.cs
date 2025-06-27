using HealthTracker.DataExchange.Entities;
using HealthTracker.Entities.Food;

namespace HealthTracker.DataExchange.Interfaces
{
    public interface IMealExporter
    {
        event EventHandler<ExportEventArgs<ExportableMeal>> RecordExport;
        Task ExportAsync(string file);
        Task ExportAsync(IEnumerable<Meal> items, string file);
    }
}

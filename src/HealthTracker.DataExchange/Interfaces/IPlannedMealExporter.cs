using HealthTracker.DataExchange.Entities;
using HealthTracker.Entities.Food;

namespace HealthTracker.DataExchange.Interfaces
{
    public interface IPlannedMealExporter
    {
        event EventHandler<ExportEventArgs<ExportablePlannedMeal>> RecordExport;
        Task ExportAsync(int personId, DateTime? from, DateTime? to, string file);
        Task ExportAsync(IEnumerable<PlannedMeal> meals, string file);
    }
}

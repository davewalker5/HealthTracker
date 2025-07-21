using HealthTracker.DataExchange.Entities;
using HealthTracker.Entities.Food;

namespace HealthTracker.DataExchange.Interfaces
{
    public interface IMealFoodItemExporter
    {
        event EventHandler<ExportEventArgs<ExportableMealFoodItem>> RecordExport;
        Task ExportAsync(string file);
        Task ExportAsync(IEnumerable<MealFoodItem> items, string file);
    }
}

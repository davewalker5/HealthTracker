using HealthTracker.DataExchange.Entities;
using HealthTracker.Entities.Food;

namespace HealthTracker.DataExchange.Interfaces
{
    public interface IFoodItemExporter
    {
        event EventHandler<ExportEventArgs<ExportableFoodItem>> RecordExport;
        Task ExportAsync(string file);
        Task ExportAsync(IEnumerable<FoodItem> items, string file);
    }
}

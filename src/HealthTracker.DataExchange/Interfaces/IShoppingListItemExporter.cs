using HealthTracker.DataExchange.Entities;
using HealthTracker.Entities.Food;

namespace HealthTracker.DataExchange.Interfaces
{
    public interface IShoppingListItemExporter
    {
        event EventHandler<ExportEventArgs<ExportableShoppingListItem>> RecordExport;
        Task ExportAsync(int personId, DateTime from, DateTime to, string file);
        Task ExportAsync(IEnumerable<ShoppingListItem> meals, string file);
    }
}

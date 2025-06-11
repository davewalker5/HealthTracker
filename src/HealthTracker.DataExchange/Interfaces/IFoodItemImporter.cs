using HealthTracker.DataExchange.Entities;

namespace HealthTracker.DataExchange.Interfaces
{
    public interface IFoodItemImporter : ICsvImporter<ExportableFoodItem>
    {
    }
}

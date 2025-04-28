using HealthTracker.DataExchange.Entities;

namespace HealthTracker.DataExchange.Interfaces
{
    public interface IPersonImporter : ICsvImporter<ExportablePerson>
    {
    }
}

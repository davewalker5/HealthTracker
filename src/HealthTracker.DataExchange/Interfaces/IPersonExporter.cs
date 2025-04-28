using HealthTracker.DataExchange.Entities;
using HealthTracker.Entities.Identity;

namespace HealthTracker.DataExchange.Interfaces
{
    public interface IPersonExporter
    {
        event EventHandler<ExportEventArgs<ExportablePerson>> RecordExport;
        void ExportAsync(IEnumerable<Person> people, string file);
    }
}

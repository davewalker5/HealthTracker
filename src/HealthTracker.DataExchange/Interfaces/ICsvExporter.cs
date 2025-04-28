using HealthTracker.DataExchange.Entities;

namespace HealthTracker.DataExchange.Interfaces
{
    public interface ICsvExporter<T> where T : class
    {
        event EventHandler<ExportEventArgs<T>> RecordExport;
        void Export(IEnumerable<T> entities, string fileName, char separator);
    }
}

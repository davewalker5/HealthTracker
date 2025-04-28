using HealthTracker.DataExchange.Entities;

namespace HealthTracker.DataExchange.Interfaces
{
    public interface ICsvImporter<T> where T : class
    {
        event EventHandler<ImportEventArgs<T>> RecordImport;
        Task ImportAsync(IEnumerable<string> records);
        Task ImportAsync(string file);
    }
}
namespace HealthTracker.Client.Interfaces
{
    public interface IDataExporter
    {
        Task ExportAsync(int personId, DateTime? from, DateTime? to, string fileName);
    }
}
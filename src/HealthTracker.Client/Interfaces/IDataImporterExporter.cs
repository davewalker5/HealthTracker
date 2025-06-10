namespace HealthTracker.Client.Interfaces
{
    public interface IDataImporterExporter : IImporterExporter
    {
        Task ExportAsync(string fileName);
    }
}
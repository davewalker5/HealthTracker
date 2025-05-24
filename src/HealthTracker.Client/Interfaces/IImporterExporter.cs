namespace HealthTracker.Client.Interfaces
{
    public interface IImporterExporter
    {
        Task ImportFromFileContentAsync(string content);
        Task ImportFromFileAsync(string filePath);
        Task ExportAsync(int personId, DateTime? from, DateTime? to, string fileName);
    }
}
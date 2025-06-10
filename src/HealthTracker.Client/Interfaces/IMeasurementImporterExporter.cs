namespace HealthTracker.Client.Interfaces
{
    public interface IMeasurementImporterExporter : IImporterExporter
    {
        Task ExportAsync(int personId, DateTime? from, DateTime? to, string fileName);
    }
}
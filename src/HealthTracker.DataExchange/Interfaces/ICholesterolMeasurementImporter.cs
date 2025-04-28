using HealthTracker.DataExchange.Entities;

namespace HealthTracker.DataExchange.Interfaces
{
    public interface ICholesterolMeasurementImporter : ICsvImporter<ExportableCholesterolMeasurement>
    {
    }
}

using HealthTracker.DataExchange.Entities;

namespace HealthTracker.DataExchange.Interfaces
{
    public interface IBloodPressureMeasurementImporter : ICsvImporter<ExportableBloodPressureMeasurement>
    {
    }
}

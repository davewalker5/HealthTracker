using HealthTracker.DataExchange.Entities;

namespace HealthTracker.DataExchange.Interfaces
{
    public interface IBloodOxygenSaturationMeasurementImporter : ICsvImporter<ExportableBloodOxygenSaturationMeasurement>
    {
    }
}

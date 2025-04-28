using HealthTracker.DataExchange.Entities;

namespace HealthTracker.DataExchange.Interfaces
{
    public interface IBloodGlucoseMeasurementImporter : ICsvImporter<ExportableBloodGlucoseMeasurement>
    {
    }
}

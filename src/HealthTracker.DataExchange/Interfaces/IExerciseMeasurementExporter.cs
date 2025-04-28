using HealthTracker.DataExchange.Entities;
using HealthTracker.Entities.Measurements;

namespace HealthTracker.DataExchange.Interfaces
{
    public interface IExerciseMeasurementExporter
    {
        event EventHandler<ExportEventArgs<ExportableExerciseMeasurement>> RecordExport;
        Task ExportAsync(int personId, DateTime? from, DateTime? to, string file);
        Task ExportAsync(IEnumerable<ExerciseMeasurement> measurements, string file);
    }
}

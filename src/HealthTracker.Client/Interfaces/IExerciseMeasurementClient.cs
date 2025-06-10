using HealthTracker.Client.ApiClient;
using HealthTracker.Entities.Measurements;

namespace HealthTracker.Client.Interfaces
{
    public interface IExerciseMeasurementClient : IDateBasedEntityRetriever<ExerciseMeasurement>, IMeasurementImporterExporter
    {
        Task<ExerciseMeasurement> AddAsync(
            int personId,
            int activityId,
            DateTime? date,
            int duration,
            decimal? distance,
            int calories,
            int minimumHeartRate,
            int maximumHeartRate);

        Task DeleteAsync(int id);

        Task<ExerciseMeasurement> UpdateAsync(
            int id,
            int personId,
            int activityId,
            DateTime? date,
            int duration,
            decimal? distance,
            int calories,
            int minimumHeartRate,
            int maximumHeartRate);

        Task<IEnumerable<ExerciseSummary>> SummariseAsync(int personId, int? activityTypeId, DateTime from, DateTime to);
    }
}
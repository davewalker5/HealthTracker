using HealthTracker.Client.ApiClient;
using HealthTracker.Entities.Measurements;

namespace HealthTracker.Client.Interfaces
{
    public interface IExerciseMeasurementClient : IDateBasedEntityRetriever<ExerciseMeasurement>
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
        Task ExportAsync(int personId, DateTime? from, DateTime? to, string fileName);
        Task ImportAsync(string filePath);

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
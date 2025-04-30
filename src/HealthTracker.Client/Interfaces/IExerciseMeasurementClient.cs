using HealthTracker.Entities.Measurements;

namespace HealthTracker.Client.Interfaces
{
    public interface IExerciseMeasurementClient
    {
        Task<ExerciseMeasurement> AddExerciseMeasurementAsync(
            int personId,
            int activityId,
            DateTime? date,
            int duration,
            decimal? distance,
            int calories,
            int minimumHeartRate,
            int maximumHeartRate);

        Task DeleteExerciseMeasurementAsync(int id);
        Task ExportExerciseMeasurementsAsync(int personId, DateTime? from, DateTime? to, string fileName);
        Task ImportExerciseMeasurementsAsync(string filePath);
        Task<List<ExerciseMeasurement>> ListExerciseMeasurementsAsync(int personId, DateTime? from, DateTime? to);

        Task<ExerciseMeasurement> UpdateExerciseMeasurementAsync(
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
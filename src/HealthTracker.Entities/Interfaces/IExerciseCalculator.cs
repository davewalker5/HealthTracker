using HealthTracker.Entities.Measurements;

namespace HealthTracker.Entities.Interfaces
{
    public interface IExerciseCalculator
    {
        IEnumerable<ExerciseSummary> Summarise(IEnumerable<ExerciseMeasurement> measurements);
        Task<IEnumerable<ExerciseSummary>> SummariseAsync(int personId, DateTime from, DateTime to);
        Task<IEnumerable<ExerciseSummary>> SummariseAsync(int personId, int activityTypeId, DateTime from, DateTime to);
        Task<IEnumerable<ExerciseSummary>> SummariseAsync(int personId, int days);
        Task<IEnumerable<ExerciseSummary>> SummariseAsync(int personId, int activityTypeId, int days);
    }
}
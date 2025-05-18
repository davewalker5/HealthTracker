using HealthTracker.Entities.Measurements;
using System.Linq.Expressions;

namespace HealthTracker.Entities.Interfaces
{
    public interface IExerciseMeasurementManager
    {
        Task<ExerciseMeasurement> AddAsync(
            int personId,
            int activityTypeId,
            DateTime date,
            int duration,
            decimal? distance,
            int calories,
            int minimumHeartRate,
            int maximumHeartRate);

        Task DeleteAsync(int id);

        Task<List<ExerciseMeasurement>> ListAsync(Expression<Func<ExerciseMeasurement, bool>> predicate, int pageNumber, int pageSize);

        Task<ExerciseMeasurement> UpdateAsync(
            int id,
            int personId,
            int activityTypeId,
            DateTime date,
            int duration,
            decimal? distance,
            int calories,
            int minimumHeartRate,
            int maximumHeartRate);
    }
}

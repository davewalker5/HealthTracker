using HealthTracker.Entities.Food;
using System.Linq.Expressions;

namespace HealthTracker.Entities.Interfaces
{
    public interface IMealConsumptionMeasurementManager
    {
        Task<List<MealConsumptionMeasurement>> ListAsync(Expression<Func<MealConsumptionMeasurement, bool>> predicate, int pageNumber, int pageSize);

        Task<MealConsumptionMeasurement> AddAsync(
            int personId,
            int mealId,
            DateTime date,
            decimal quantity);

        Task<MealConsumptionMeasurement> UpdateAsync(
            int id,
            int personId,
            int mealId,
            DateTime date,
            decimal quantity);

        Task DeleteAsync(int id);
    }
}
using HealthTracker.Entities.Measurements;
using System.Linq.Expressions;
using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.Entities.Interfaces
{
    public interface IBeverageConsumptionMeasurementManager
    {
        Task<List<BeverageConsumptionMeasurement>> ListAsync(Expression<Func<BeverageConsumptionMeasurement, bool>> predicate, int pageNumber, int pageSize);

        Task<BeverageConsumptionMeasurement> AddAsync(
            int personId,
            int beverageId,
            DateTime date,
            TempBeverageMeasure measure,
            int quantity,
            decimal volume,
            decimal abv);

        Task<BeverageConsumptionMeasurement> UpdateAsync(
            int id,
            int personId,
            int beverageId,
            DateTime date,
            TempBeverageMeasure measure,
            int quantity,
            decimal volume,
            decimal abv);

        Task DeleteAsync(int id);
    }
}
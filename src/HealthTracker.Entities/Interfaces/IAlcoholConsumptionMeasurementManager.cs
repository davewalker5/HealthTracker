using HealthTracker.Entities.Measurements;
using System.Linq.Expressions;
using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.Entities.Interfaces
{
    public interface IAlcoholConsumptionMeasurementManager
    {
        Task<List<AlcoholConsumptionMeasurement>> ListAsync(Expression<Func<AlcoholConsumptionMeasurement, bool>> predicate, int pageNumber, int pageSize);

        Task<AlcoholConsumptionMeasurement> AddAsync(
            int personId,
            int beverageId,
            DateTime date,
            AlcoholMeasure measure,
            int quantity,
            decimal abv);

        Task<AlcoholConsumptionMeasurement> UpdateAsync(
            int id,
            int personId,
            int beverageId,
            DateTime date,
            AlcoholMeasure measure,
            int quantity,
            decimal abv);

        Task DeleteAsync(int id);
    }
}
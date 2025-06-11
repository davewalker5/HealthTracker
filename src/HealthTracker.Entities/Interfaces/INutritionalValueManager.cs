using HealthTracker.Entities.Food;
using System.Linq.Expressions;

namespace HealthTracker.Entities.Interfaces
{
    public interface INutritionalValueManager
    {
        Task<NutritionalValue> AddAsync(
            decimal? calories,
            decimal? fat,
            decimal? saturatedFat,
            decimal? protein,
            decimal? carbohydrates,
            decimal? sugar,
            decimal? fibre);

        Task<NutritionalValue> GetAsync(Expression<Func<NutritionalValue, bool>> predicate);

        Task<NutritionalValue> UpdateAsync(
            int id,
            decimal? calories,
            decimal? fat,
            decimal? saturatedFat,
            decimal? protein,
            decimal? carbohydrates,
            decimal? sugar,
            decimal? fibre);

        Task DeleteAsync(int id);
    }
}
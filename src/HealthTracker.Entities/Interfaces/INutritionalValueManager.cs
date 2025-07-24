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

        Task<NutritionalValue> AddAsync(NutritionalValue nutrition);
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
        void CheckNutritionalValueExists(int? nutritionalValueId);
        NutritionalValue CalculateNutritionalValues(NutritionalValue baseNutrition, decimal quantity);
        Task<NutritionalValue> CreateOrUpdateNutritionalValueAsync(int? id, NutritionalValue template);
        NutritionalValue CalculateTotalNutritionalValue(IEnumerable<MealFoodItem> items);
    }
}
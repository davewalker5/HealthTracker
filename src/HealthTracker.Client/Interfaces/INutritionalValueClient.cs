using HealthTracker.Entities.Food;

namespace HealthTracker.Client.Interfaces
{
    public interface INutritionalValueClient
    {
        Task<NutritionalValue> AddAsync(
            decimal? calories,
            decimal? fat,
            decimal? saturatedFat,
            decimal? protein,
            decimal? carbohydrates,
            decimal? sugar,
            decimal? fibre);

        Task<NutritionalValue> GetAsync(int id);

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
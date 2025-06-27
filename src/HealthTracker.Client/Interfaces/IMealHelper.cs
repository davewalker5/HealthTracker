using HealthTracker.Entities.Food;

namespace HealthTracker.Client.Interfaces
{
    public interface IMealHelper
    {
        Task<Meal> GetAsync(int id);
        Task<List<Meal>> ListAsync(int foodCategoryId, int pageNumber, int pageSize);
        Task DeleteAsync(int id);
        Task<Meal> AddAsync(Meal template);
        Task<Meal> UpdateAsync(Meal template);
    }
}
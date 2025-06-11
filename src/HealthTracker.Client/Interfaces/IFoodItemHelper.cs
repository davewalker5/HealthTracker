using HealthTracker.Entities.Food;

namespace HealthTracker.Client.Interfaces
{
    public interface IFoodItemHelper
    {
        Task<FoodItem> GetAsync(int id);
        Task<List<FoodItem>> ListAsync(int foodCategoryId, int pageNumber, int pageSize);
        Task DeleteAsync(int id);
        Task<FoodItem> AddAsync(FoodItem template);
        Task<FoodItem> UpdateAsync(FoodItem template);
    }
}
using HealthTracker.Entities.Food;

namespace HealthTracker.Client.Interfaces
{
    public interface IFoodCategoryClient
    {
        Task<FoodCategory> AddAsync(string name);
        Task DeleteAsync(int id);
        Task<List<FoodCategory>> ListAsync(int pageNumber, int pageSize);
        Task<FoodCategory> UpdateAsync(int id, string name);
    }
}
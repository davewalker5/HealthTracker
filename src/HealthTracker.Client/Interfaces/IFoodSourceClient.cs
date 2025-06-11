using HealthTracker.Entities.Food;

namespace HealthTracker.Client.Interfaces
{
    public interface IFoodSourceClient
    {
        Task<FoodSource> AddAsync(string name);
        Task DeleteAsync(int id);
        Task<List<FoodSource>> ListAsync(int pageNumber, int pageSize);
        Task<FoodSource> UpdateAsync(int id, string name);
    }
}
using HealthTracker.Entities.Measurements;

namespace HealthTracker.Client.Interfaces
{
    public interface IBeverageClient
    {
        Task<Beverage> AddAsync(string name, decimal typicalABV, bool isHydrating, bool isAlcohol);
        Task DeleteAsync(int id);
        Task<List<Beverage>> ListAsync(int pageNumber, int pageSize);
        Task<Beverage> UpdateAsync(int id, string name, decimal typicalABV, bool isHydrating, bool isAlcohol);
    }
}
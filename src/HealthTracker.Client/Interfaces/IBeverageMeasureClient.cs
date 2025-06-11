using HealthTracker.Entities.Food;

namespace HealthTracker.Client.Interfaces
{
    public interface IBeverageMeasureClient
    {
        Task<BeverageMeasure> AddAsync(string description, decimal volume);
        Task DeleteAsync(int id);
        Task<List<BeverageMeasure>> ListAsync(int pageNumber, int pageSize);
        Task<BeverageMeasure> UpdateAsync(int id, string description, decimal volume);
    }
}
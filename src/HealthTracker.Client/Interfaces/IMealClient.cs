using HealthTracker.Entities.Food;

namespace HealthTracker.Client.Interfaces
{
    public interface IMealClient : IEntityRetriever<Meal>, IDataImporterExporter
    {
        Task<Meal> AddAsync(
            string name,
            decimal portion,
            int? nutritionalValueId);

        Task<Meal> UpdateAsync(
            int id,
            string name,
            decimal portion,
            int? nutritionalValueId);

        Task DeleteAsync(int id);
        Task<List<Meal>> ListAsync(int pageNumber, int pageSize);
    }
}
using HealthTracker.Entities.Food;

namespace HealthTracker.Client.Interfaces
{
    public interface IMealClient : IEntityRetriever<Meal>, IDataImporterExporter
    {
        Task<Meal> AddAsync(
            string name,
            int portions,
            int foodSourceId,
            int? nutritionalValueId);

        Task<Meal> UpdateAsync(
            int id,
            string name,
            int portions,
            int foodSourceId,
            int? nutritionalValueId);

        Task DeleteAsync(int id);
        Task<List<Meal>> ListAsync(int foodSourceId, int pageNumber, int pageSize);
    }
}
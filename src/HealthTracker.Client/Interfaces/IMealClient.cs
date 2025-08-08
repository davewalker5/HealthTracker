using HealthTracker.Entities.Food;

namespace HealthTracker.Client.Interfaces
{
    public interface IMealClient : IEntityRetriever<Meal>, IDataImporterExporter
    {
        Task<Meal> AddAsync(
            string name,
            int portions,
            int foodSourceId,
            string reference,
            int? nutritionalValueId);

        Task<Meal> UpdateAsync(
            int id,
            string name,
            int portions,
            int foodSourceId,
            string reference,
            int? nutritionalValueId);

        Task UpdateAllNutritionalValues();
        Task DeleteAsync(int id);
        Task<List<Meal>> ListAsync(int foodSourceId, int pageNumber, int pageSize);
        Task<List<Meal>> SearchAsync(
            int? foodSourceId,
            int? foodCategoryId,
            string mealNameSubstring,
            string foodItemNameSubstring,
            int pageNumber,
            int pageSize);
    }
}
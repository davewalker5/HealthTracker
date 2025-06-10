using HealthTracker.Entities.Food;

namespace HealthTracker.Client.Interfaces
{
    public interface IFoodItemClient : IEntityRetriever<FoodItem>, IDataImporterExporter
    {
        Task<FoodItem> AddAsync(
            string name,
            decimal portion,
            int foodCategoryId,
            int? nutritionalValueId);

        Task<FoodItem> UpdateAsync(
            int id,
            string name,
            decimal portion,
            int foodCategoryId,
            int? nutritionalValueId);

        Task DeleteAsync(int id);
        Task<List<FoodItem>> ListAsync(int foodCategoryId, int pageNumber, int pageSize);
    }
}
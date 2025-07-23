using HealthTracker.Entities.Food;

namespace HealthTracker.Client.Interfaces
{
    public interface IMealFoodItemClient : IEntityRetriever<MealFoodItem>, IDataImporterExporter
    {
        Task<MealFoodItem> AddAsync(int mealId, int foodItemId, decimal quantity, int? nutritionalValueId);
        Task<MealFoodItem> UpdateAsync(int id, int mealId, int foodItemId, decimal quantity, int? nutritionalValueId);
        Task DeleteAsync(int id);
        Task<List<MealFoodItem>> ListAsync(int mealId);
    }
}
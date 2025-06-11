using HealthTracker.Mvc.Entities;
using HealthTracker.Mvc.Models;

namespace HealthTracker.Mvc.Interfaces
{
    public interface IFoodCategoryFilterGenerator
    {
        Task<FoodCategoryFilterViewModel> Create(int foodCategoryId, ViewFlags flags);
        Task PopulateFoodCategoryList(FoodCategoryFilterViewModel model);
    }
}
using HealthTracker.Mvc.Entities;
using HealthTracker.Mvc.Models;

namespace HealthTracker.Mvc.Interfaces
{
    public interface IFoodSourceFilterGenerator
    {
        Task<FoodSourceFilterViewModel> Create(int foodSourceId, ViewFlags flags);
        Task PopulateFoodSourceList(FoodSourceFilterViewModel model);
    }
}
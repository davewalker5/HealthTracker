using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Interfaces
{
    public interface IFoodItemListGenerator
    {
        Task<IList<SelectListItem>> Create(int foodCategoryId);
    }
}
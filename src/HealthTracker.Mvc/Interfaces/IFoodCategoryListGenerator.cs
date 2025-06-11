using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Interfaces
{
    public interface IFoodCategoryListGenerator
    {
        Task<IList<SelectListItem>> Create();
    }
}
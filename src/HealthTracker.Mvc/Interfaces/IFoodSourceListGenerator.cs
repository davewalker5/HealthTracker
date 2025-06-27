using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Interfaces
{
    public interface IFoodSourceListGenerator
    {
        Task<IList<SelectListItem>> Create();
    }
}
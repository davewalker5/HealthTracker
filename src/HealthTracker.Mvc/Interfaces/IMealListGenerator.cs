using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Interfaces
{
    public interface IMealListGenerator
    {
        Task<IList<SelectListItem>> Create(int foodSourceId);
    }
}
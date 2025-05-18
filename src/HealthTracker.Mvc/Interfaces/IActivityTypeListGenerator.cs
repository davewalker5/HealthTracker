using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Interfaces
{
    public interface IActivityTypeListGenerator
    {
        Task<IList<SelectListItem>> Create();
    }
}
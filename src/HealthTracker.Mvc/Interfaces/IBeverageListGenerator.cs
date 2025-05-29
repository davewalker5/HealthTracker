using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Interfaces
{
    public interface IBeverageListGenerator
    {
        Task<IList<SelectListItem>> Create();
    }
}
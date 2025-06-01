using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Interfaces
{
    public interface IBeverageMeasureListGenerator
    {
        Task<IList<SelectListItem>> Create();
    }
}
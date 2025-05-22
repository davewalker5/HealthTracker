using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Interfaces
{
    public interface IMedicationListGenerator
    {
        Task<IList<SelectListItem>> Create(int personId, int associationId);
    }
}
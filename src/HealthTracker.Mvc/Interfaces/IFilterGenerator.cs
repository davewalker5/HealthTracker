using HealthTracker.Mvc.Models;

namespace HealthTracker.Mvc.Interfaces
{
    public interface IFilterGenerator
    {
        Task<FiltersViewModel> Create();
        Task PopulatePersonList(FiltersViewModel model);
    }
}
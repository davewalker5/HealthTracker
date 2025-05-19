using HealthTracker.Mvc.Models;

namespace HealthTracker.Mvc.Interfaces
{
    public interface IFilterGenerator
    {
        Task<FiltersViewModel> Create();
        Task<FiltersViewModel> Create(int personId);
        Task<FiltersViewModel> Create(int personId, DateTime? from, DateTime? to);
        Task PopulatePersonList(FiltersViewModel model);
    }
}
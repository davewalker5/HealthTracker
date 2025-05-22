using HealthTracker.Mvc.Models;

namespace HealthTracker.Mvc.Interfaces
{
    public interface IFilterGenerator
    {
        Task<PersonFilterViewModel> Create(int personId, bool showAddButton);
        Task<FiltersViewModel> Create(int personId, DateTime? from, DateTime? to, bool showAddButton);
        Task PopulatePersonList(PersonFilterViewModel model);
    }
}
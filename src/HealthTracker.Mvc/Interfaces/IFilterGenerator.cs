using HealthTracker.Mvc.Entities;
using HealthTracker.Mvc.Models;

namespace HealthTracker.Mvc.Interfaces
{
    public interface IFilterGenerator
    {
        Task<PersonFilterViewModel> Create(int personId, ViewFlags flags);
        Task<FiltersViewModel> Create(int personId, DateTime? from, DateTime? to, ViewFlags flags);
        Task PopulatePersonList(PersonFilterViewModel model);
    }
}
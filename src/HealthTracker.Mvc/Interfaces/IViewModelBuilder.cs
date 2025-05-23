using HealthTracker.Client.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.Mvc.Entities;
using HealthTracker.Mvc.Models;

namespace HealthTracker.Mvc.Interfaces
{
    public interface IViewModelBuilder
    {
        Task<L> CreateFilteredListViewModel<C, L, M>(C client, int personId, int measurementId, IEnumerable<M> measurements, DateTime from, DateTime to, string message, ViewFlags flags)
            where C : IDateBasedEntityRetriever<M>
            where L : FilteredViewModelBase<M>, new()
            where M : class, new();

        Task<PersonMedicationListViewModel> CreatePersonMedicationListViewModel(int personId, string message, ViewFlags flags);
        Task<WeightListViewModel> CreateWeightListViewModel(int personId, int measurementId, IEnumerable<WeightMeasurement> measurements, DateTime from, DateTime to, string message, ViewFlags flags);
    }
}
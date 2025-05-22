using HealthTracker.Client.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.Mvc.Models;

namespace HealthTracker.Mvc.Interfaces
{
    public interface IViewModelBuilder
    {
        Task<L> CreateFilteredListViewModel<C, L, M>(C client, int personId, int measurementId, IEnumerable<M> measurements, DateTime from, DateTime to, string message, bool editable, bool showAddButton)
            where C : IDateBasedEntityRetriever<M>
            where L : FilteredViewModelBase<M>, new()
            where M : class, new();

        Task<PersonMedicationListViewModel> CreatePersonMedicationListViewModel(int personId, string message, bool includeInactive, bool editable);
        Task<WeightListViewModel> CreateWeightListViewModel(int personId, int measurementId, IEnumerable<WeightMeasurement> measurements, DateTime from, DateTime to, string message, bool editable, bool showAddButton);
        Task<ExerciseListViewModel> CreateExerciseListViewModel(int personId, int measurementId, IEnumerable<ExerciseMeasurement> measurements, DateTime from, DateTime to, string message, bool editable, bool showAddButton);
    }
}
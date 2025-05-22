using HealthTracker.Entities.Measurements;

namespace HealthTracker.Mvc.Models
{
    public class ExerciseListViewModel : FilteredViewModelBase<ExerciseMeasurement>
    {
        public IEnumerable<ExerciseMeasurement> Measurements => Entities;
    }
}
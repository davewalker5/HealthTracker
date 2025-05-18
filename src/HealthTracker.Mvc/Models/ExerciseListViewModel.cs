using HealthTracker.Entities.Measurements;

namespace HealthTracker.Mvc.Models
{
    public class ExerciseListViewModel : MeasurementListViewModelBase<ExerciseMeasurement>
    {
        public IEnumerable<ExerciseMeasurement> Measurements => Entities;
    }
}
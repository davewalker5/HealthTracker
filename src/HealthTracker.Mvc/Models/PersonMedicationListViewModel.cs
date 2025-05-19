using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Medications;

namespace HealthTracker.Mvc.Models
{
    public class PersonMedicationListViewModel : MeasurementListViewModelBase<PersonMedication>
    {
        public IEnumerable<PersonMedication> Associations => Entities;
        public IHealthTrackerApplicationSettings Settings { get; set; }
    }
}
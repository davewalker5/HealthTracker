using HealthTracker.Entities.Medications;

namespace HealthTracker.Mvc.Models
{
    public class MedicationListViewModel : ListViewModelBase<Medication>
    {
        public IEnumerable<Medication> Medications => Entities;
    }
}
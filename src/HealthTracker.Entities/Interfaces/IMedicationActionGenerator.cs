using HealthTracker.Entities.Medications;

namespace HealthTracker.Entities.Interfaces
{
    public interface IMedicationActionGenerator
    {
        void DetermineActions(PersonMedication association);
        void DetermineActions(IEnumerable<PersonMedication> associations);
    }
}

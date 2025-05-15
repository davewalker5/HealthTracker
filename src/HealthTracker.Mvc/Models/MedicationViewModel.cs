using HealthTracker.Entities.Medications;

namespace HealthTracker.Mvc.Models
{
    public class MedicationViewModel
    {
        public Medication Medication { get; set; } = new();
    }
}
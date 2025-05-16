using HealthTracker.Entities.Medications;

namespace HealthTracker.Mvc.Models
{
    public class MedicationViewModel
    {
        public Medication Medication { get; set; } = new();
        public string Action { get; set; }

        public MedicationViewModel()
        {
            Medication.Id = 0;
            Medication.Name = "";
        }
    }
}
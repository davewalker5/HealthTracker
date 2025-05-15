namespace HealthTracker.Mvc.Models
{
    public class AddMedicationViewModel : MedicationViewModel
    {
        public string Message { get; set; } = "";

        public AddMedicationViewModel()
        {
            Medication.Id = 0;
            Medication.Name = "";
        }
    }
}
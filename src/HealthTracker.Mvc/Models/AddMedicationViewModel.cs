namespace HealthTracker.Mvc.Models
{
    public class AddMedicationViewModel : MedicationViewModel
    {
        public string Message { get; set; }

        public AddMedicationViewModel()
            => Clear();

        public void Clear()
        {
            Medication.Id = 0;
            Medication.Name = "";
        }
    }
}
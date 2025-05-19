using HealthTracker.Entities.Medications;

namespace HealthTracker.Mvc.Models
{
    public class PersonMedicationViewModel : SelectedFiltersViewModel
    {
        public PersonMedication Association { get; set; } = new();
        public string Class { get; set; }
        public string Action { get; set; }

        public PersonMedicationViewModel()
        {
            // TODO : Initialise the model
            Class = "";
        }
    }
}
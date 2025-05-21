using HealthTracker.Entities.Medications;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Models
{
    public class PersonMedicationViewModel : SelectedFiltersViewModel
    {
        public IList<SelectListItem> Medications { get; set; } = [];
        public PersonMedication Association { get; set; } = new();
        public string Class { get; set; }
        public string Action { get; set; }

        public PersonMedicationViewModel()
        {
            Class = "";
            Association.Id = 0;
            Association.PersonId = 0;
            Association.MedicationId = 0;
            Association.Stock = 0;
            Association.DailyDose = 0;
            Association.LastTaken = null;
            Association.Active = true;
        }
    }
}
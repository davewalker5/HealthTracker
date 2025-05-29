using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using HealthTracker.Entities.Measurements;
using HealthTracker.Mvc.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using HealthTracker.Enumerations.Enumerations;
using HealthTracker.Enumerations.Extensions;

namespace HealthTracker.Mvc.Models
{
    public class AlcoholCalculationViewModel : SelectedFiltersViewModel
    {
        public List<SelectListItem> Measures { get; private set; } = [];
        public string MeasureName { get { return Measure.ToName(); } }
        public string Result { get; set; }

        [DisplayName("Measure")]
        public AlcoholMeasure Measure { get; set; }

        [DisplayName("Quantity")]
        [Required(ErrorMessage = "You must enter a quantity")]
        public decimal Quantity { get; set; }

        [DisplayName("ABV %")]
        [Range(1.0, 100.0, ErrorMessage = "{0} must be between {1} and {2}")]
        public decimal ABV { get; set; }

        public AlcoholCalculationViewModel()
        {
            foreach (var measure in Enum.GetValues<AlcoholMeasure>())
            {
                Measures.Add(new SelectListItem() { Text = $"{measure.ToName()}", Value = measure.ToString() });
            }
        }
    }
}
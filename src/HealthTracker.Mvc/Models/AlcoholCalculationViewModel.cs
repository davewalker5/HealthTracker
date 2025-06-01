using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.Mvc.Models
{
    public class AlcoholCalculationViewModel : SelectedFiltersViewModel
    {
        public IList<SelectListItem> Measures { get; set; } = [];
        public string Result { get; set; }

        [DisplayName("Measure")]
        public decimal Measure { get; set; }

        [DisplayName("Volume")]
        [Range(1.0, float.MaxValue, ErrorMessage = "{0} must be >= {1}")]
        public decimal Volume { get; set; }

        [DisplayName("Quantity")]
        [Required(ErrorMessage = "You must enter a quantity")]
        public decimal Quantity { get; set; }

        [DisplayName("ABV %")]
        [Range(1.0, 100.0, ErrorMessage = "{0} must be between {1} and {2}")]
        public decimal ABV { get; set; }
    }
}
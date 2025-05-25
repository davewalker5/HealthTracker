using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using HealthTracker.Entities.Measurements;
using HealthTracker.Mvc.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Models
{
    public class AlcoholCalculationViewModel : SelectedFiltersViewModel
    {
        protected readonly Dictionary<AlcoholPortionSize, string> _typeNameMap = new()
        {
            { AlcoholPortionSize.None, "" },
            { AlcoholPortionSize.Pint, "Pint" },
            { AlcoholPortionSize.LargeGlass, "Large Glass" },
            { AlcoholPortionSize.MediumGlass, "Medium Glass" },
            { AlcoholPortionSize.SmallGlass, "Small Glass" },
            { AlcoholPortionSize.Shot, "Shot" }
        };

        public List<SelectListItem> PortionSizes { get; private set; } = [];
        public string PortionSizeName { get { return _typeNameMap[Portion]; } }
        public string Result { get; set; }

        [DisplayName("Portion")]
        public AlcoholPortionSize Portion { get; set; }

        [DisplayName("Quantity")]
        [Required(ErrorMessage = "You must enter a quantity")]
        public decimal Quantity { get; set; }

        [DisplayName("ABV %")]
        [Range(1.0, 100.0, ErrorMessage = "{0} must be between {1} and {2}")]
        public decimal ABV { get; set; }

        public AlcoholCalculationViewModel()
        {
            foreach (var portionSize in Enum.GetValues<AlcoholPortionSize>())
            {
                var portionSizeName = _typeNameMap[portionSize];
                PortionSizes.Add(new SelectListItem() { Text = $"{portionSizeName}", Value = portionSize.ToString() });
            }
        }
    }
}
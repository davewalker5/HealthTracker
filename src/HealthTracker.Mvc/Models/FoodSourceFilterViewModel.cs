using System.ComponentModel;
using HealthTracker.Entities.Food;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Models
{
    public class FoodSourceFilterViewModel
    {
        public IList<SelectListItem> Sources { get; set; } = [];
        public bool ShowAddButton { get; set; }
        public bool ShowExportButton { get; set; }

        [DisplayName("Food Source")]
        public int FoodSourceId { get; set; }

        public FoodSource SelectedSource { get; set; }
    }
}
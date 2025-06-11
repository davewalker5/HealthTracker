using System.ComponentModel;
using HealthTracker.Entities.Food;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Models
{
    public class FoodCategoryFilterViewModel
    {
        public IList<SelectListItem> Categories { get; set; } = [];
        public bool ShowAddButton { get; set; }
        public bool ShowExportButton { get; set; }

        [DisplayName("Food Category")]
        public int FoodCategoryId { get; set; }

        public FoodCategory SelectedCategory { get; set; }
    }
}
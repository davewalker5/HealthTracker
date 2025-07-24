using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Models
{
    public class FoodItemsDropDownViewModel
    {
        public IList<SelectListItem> Items { get; set; } = [];

        [DisplayName("Food Item")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a food item")]
        public int FoodItemId { get; set; }
    }
}
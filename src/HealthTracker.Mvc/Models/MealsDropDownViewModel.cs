using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Models
{
    public class MealsDropDownViewModel
    {
        public IList<SelectListItem> Meals { get; set; } = [];

        [DisplayName("Meal")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a food source")]
        public int MealId { get; set; }

        public string TargetField { get; set; }
    }
}
using HealthTracker.Entities.Food;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Models
{
    public class MealViewModel
    {
        public IList<SelectListItem> FoodSources { get; set; } = [];

        public Meal Meal { get; set; }

        public string Action { get; set; }

        /// <summary>
        /// Create a new, empty meal
        /// </summary>
        public void CreateMeal()
            => Meal = new()
            {
                Id = 0,
                Name = "",
                Portions = 0,
                FoodSource = new(),
                NutritionalValueId = new()
            };
    }
}
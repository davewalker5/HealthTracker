using HealthTracker.Entities.Food;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Models
{
    public class FoodItemViewModel
    {
        public IList<SelectListItem> FoodCategories { get; set; } = [];

        public FoodItem FoodItem { get; set; }

        public string Action { get; set; }

        /// <summary>
        /// Create a new, empty food item
        /// </summary>
        public void CreateItem()
            => FoodItem = new()
            {
                Id = 0,
                Name = "",
                Portion = 0,
                FoodCategory = new(),
                NutritionalValueId = new()
            };
    }
}
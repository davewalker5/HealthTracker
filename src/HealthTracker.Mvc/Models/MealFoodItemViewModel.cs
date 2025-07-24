using HealthTracker.Entities;
using HealthTracker.Entities.Food;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Models
{
    public class MealFoodItemViewModel : HealthTrackerEntityBase
    {
        public IList<SelectListItem> FoodCategories { get; set; } = [];
        public MealFoodItem Relationship { get; set; } = new();
        public string Meal { get; set; }
        public string Action { get; set; }

        /// <summary>
        /// Create a new, empty relationship for the specified meal
        /// </summary>
        /// <param name="mealId"></param>
        public void CreateRelationship(int mealId)
            => Relationship = new MealFoodItem()
            {
                Quantity = 0M,
                MealId = mealId,
                FoodItemId = 0,
                FoodItem = new()
                {
                    FoodCategoryId = 0,
                    FoodCategory = new()
                }
            };
    }
}
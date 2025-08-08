using System.ComponentModel;

namespace HealthTracker.Entities.Food
{
    public class MealSearchCriteria : HealthTrackerEntityBase
    {
        [DisplayName("Meal Food Source")]
        public int? FoodSourceId { get; set; }

        [DisplayName("Food Item Category")]
        public int? FoodCategoryId { get; set; }

        [DisplayName("Meal Name")]
        public string MealName { get; set; }

        [DisplayName("Food Item Name")]
        public string FoodItemName { get; set; }
    }
}
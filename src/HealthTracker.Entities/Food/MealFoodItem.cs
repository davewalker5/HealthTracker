using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Food
{
    [ExcludeFromCodeCoverage]
    public class MealFoodItem : HealthTrackerEntityBase
    {
        [Key]
        public int Id { get ; set; }

        [DisplayName("Meal")]
        [ForeignKey(nameof(Meal))]
        [Range(1, int.MaxValue, ErrorMessage = "You must specify a meal")]
        public int MealId { get; set; }

        [DisplayName("Food Item")]
        [ForeignKey(nameof(FoodItem))]
        [Range(1, int.MaxValue, ErrorMessage = "You must specify a food item")]
        public int FoodItemId { get; set; }
        public FoodItem FoodItem { get; set; }
    }
}
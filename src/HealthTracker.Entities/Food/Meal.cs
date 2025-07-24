using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Food
{
    [ExcludeFromCodeCoverage]
    public class Meal : NamedEntity
    {
        [DisplayName("Portions")]
        [Range(1, int.MaxValue, ErrorMessage = "{0} must be >= {1}")]
        public int Portions { get; set; }

        [DisplayName("Food Source")]
        [ForeignKey(nameof(FoodSource))]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a food source")]
        public int FoodSourceId { get; set; }
        public FoodSource FoodSource { get; set; }

        [ForeignKey(nameof(NutritionalValue))]
        public int? NutritionalValueId { get; set; }
        public NutritionalValue NutritionalValue { get; set; }

        public ICollection<MealFoodItem> MealFoodItems { get; set; }
    }
}
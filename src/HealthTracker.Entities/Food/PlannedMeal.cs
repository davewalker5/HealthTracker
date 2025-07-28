using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.Entities.Food
{
    [ExcludeFromCodeCoverage]
    public class PlannedMeal : MeasurementBase
    {
        [DisplayName("Meal Type")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a meal type")]
        public MealType MealType { get; set; }

        [DisplayName("Meal")]
        [ForeignKey(nameof(Meal))]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a meal")]
        public int MealId { get; set; }
        public Meal Meal { get; set; }
    }
}
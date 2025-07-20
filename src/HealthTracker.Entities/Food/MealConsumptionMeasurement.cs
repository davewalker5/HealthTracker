
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Food
{
    [ExcludeFromCodeCoverage]
    public class MealConsumptionMeasurement : MeasurementBase
    {
        [ForeignKey(nameof(Meal))]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a meal")]
        public int MealId { get; set; }
        public Meal Meal { get; set; }

        [DisplayName("Quantity")]
        [Range(1.0, double.MaxValue, ErrorMessage = "{0} must be >= {1}")]
        public decimal Quantity { get; set; }

        [ForeignKey(nameof(NutritionalValue))]
        public int? NutritionalValueId { get; set; }
        public NutritionalValue NutritionalValue { get; set; }
    }
}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Food
{
    [ExcludeFromCodeCoverage]
    public class FoodItem : NamedEntity
    {
        [DisplayName("Portion")]
        [Range(1.0, double.MaxValue, ErrorMessage="{0} must be >= {1}")]
        public decimal Portion { get; set; }

        [DisplayName("Food Category")]
        [ForeignKey(nameof(FoodCategory))]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a food category")]
        public int FoodCategoryId { get; set; }
        public FoodCategory FoodCategory { get; set; }

        [ForeignKey(nameof(NutritionalValue))]
        public int? NutritionalValueId { get; set; }
        public NutritionalValue NutritionalValue { get; set; }
    }
}
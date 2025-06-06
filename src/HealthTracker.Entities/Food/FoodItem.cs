using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Food
{
    [ExcludeFromCodeCoverage]
    public class FoodItem : NamedEntity
    {
        [DisplayName("Name")]
        [Range(1.0, double.MaxValue, ErrorMessage="{0} must be >= {1}")]
        public decimal Portion { get; set; }

        [ForeignKey(nameof(FoodCategory))]
        [Required(ErrorMessage="You must select a food category")]
        public int FoodCategoryId { get; set; }
        public FoodCategory FoodCategory { get; set; }

        [ForeignKey(nameof(NutritionalValue))]
        public int? NutritionalValueId { get; set; }
        public NutritionalValue NutritionalValue { get; set; }
    }
}
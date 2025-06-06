using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Food
{
    [ExcludeFromCodeCoverage]
    public class NutritionalValue
    {
        [Key]
        public int Id { get ; set; }

        [DisplayName("Calories")]
        public decimal? Calories { get; set; }

        [DisplayName("Fat")]
        public decimal? Fat { get; set; }

        [DisplayName("Saturated Fat")]
        public decimal? SaturatedFat { get; set; }

        [DisplayName("Protein")]
        public decimal? Protein { get; set; }

        [DisplayName("Carbohydrates")]
        public decimal? Carbohydrates { get; set; }

        [DisplayName("Sugar")]
        public decimal? Sugar { get; set; }

        [DisplayName("Fibre")]
        public decimal? Fibre { get; set; }

        public FoodItem FoodItem { get; set; }
    }
}
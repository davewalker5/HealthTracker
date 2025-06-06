using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Food
{
    [ExcludeFromCodeCoverage]
    public class Beverage : NamedEntity
    {
        [DisplayName("Typical ABV %")]
        [Range(0.0, double.MaxValue, ErrorMessage = "{0} must be between {1} and {2}")]
        public decimal TypicalABV { get; set; }

        [DisplayName("Hydrating")]
        public bool IsHydrating { get; set; }

        [DisplayName("Alcohol")]
        public bool IsAlcohol { get; set; }
    }
}
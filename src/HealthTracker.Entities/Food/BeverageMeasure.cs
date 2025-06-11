using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Food
{
    [ExcludeFromCodeCoverage]
    public class BeverageMeasure : NamedEntity
    {
        [DisplayName("Volume")]
        [Range(1.0, double.MaxValue, ErrorMessage = "{0} must be >= {1}")]
        public decimal Volume { get; set; }
    }
}
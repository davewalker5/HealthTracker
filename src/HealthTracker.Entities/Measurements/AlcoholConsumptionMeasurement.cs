using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.Entities.Measurements
{
    [ExcludeFromCodeCoverage]
    public class AlcoholConsumptionMeasurement : MeasurementBase
    {
        [DisplayName("Beverage")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a beverage")]
        [ForeignKey(nameof(Beverage))]
        public int BeverageId { get; set; }

        [DisplayName("Measure")]
        [Required(ErrorMessage = "You must select a measure")]
        public AlcoholMeasure Measure { get; set; }

        [DisplayName("ABV")]
        [Range(1, int.MaxValue, ErrorMessage = "{0} must be >= {1}")]
        public int Quantity { get; set; }

        [DisplayName("ABV")]
        [Range(0, 100, ErrorMessage = "{0} must be between {1} and {2}")]
        public decimal ABV { get; set; }

        [NotMapped]
        public string Beverage { get; set; }
    }
}

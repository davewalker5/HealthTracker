using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Measurements
{
    [ExcludeFromCodeCoverage]
    public class BeverageConsumptionMeasurement : MeasurementBase
    {
        [DisplayName("Beverage")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a beverage")]
        [ForeignKey(nameof(Beverage))]
        public int BeverageId { get; set; }

        [DisplayName("Quantity")]
        [Range(1, int.MaxValue, ErrorMessage = "{0} must be >= {1}")]
        public int Quantity { get; set; }

        [DisplayName("Volume")]
        [Range(1, int.MaxValue, ErrorMessage = "{0} must be >= {1}")]
        public decimal Volume { get; set; }

        [DisplayName("ABV")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} must be >= {1}")]
        public decimal ABV { get; set; }

        [NotMapped]
        public string Beverage { get; set; }

        [NotMapped]
        public decimal Units { get; set; }
    }
}

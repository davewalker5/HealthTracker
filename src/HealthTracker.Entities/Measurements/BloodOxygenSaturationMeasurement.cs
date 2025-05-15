using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Measurements
{
    [ExcludeFromCodeCoverage]
    public class BloodOxygenSaturationMeasurement : MeasurementBase
    {
        [DisplayName("% SPO2")]
        [Range(1.0, 100.0, ErrorMessage = "{0} must be between {1} and {2}")]
        public decimal Percentage { get; set; }

        [NotMapped]
        public string Assessment { get; set; }
    }
}
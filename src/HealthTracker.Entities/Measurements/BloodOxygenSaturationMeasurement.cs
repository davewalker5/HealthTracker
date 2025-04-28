using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Measurements
{
    [ExcludeFromCodeCoverage]
    public class BloodOxygenSaturationMeasurement : MeasurementBase
    {
        public decimal Percentage { get; set; }

        [NotMapped]
        public string Assessment { get; set; }
    }
}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Measurements
{
    [ExcludeFromCodeCoverage]
    public class BloodGlucoseMeasurement : MeasurementBase
    {
        [DisplayName("Level")]
        [Range(1.0, double.MaxValue, ErrorMessage = "{0} must be >= {1}")]
        public decimal Level { get ; set; }
    }
}

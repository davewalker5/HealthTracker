using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Measurements
{
    [ExcludeFromCodeCoverage]
    public class CholesterolMeasurement : MeasurementBase
    {
        public decimal Total { get ; set; }
        public decimal HDL { get; set; }
        public decimal LDL { get; set; }
        public decimal Triglycerides { get; set; }
    }
}

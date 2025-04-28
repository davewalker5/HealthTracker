using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Measurements
{
    [ExcludeFromCodeCoverage]
    public class BloodGlucoseMeasurement : MeasurementBase
    {
        public decimal Level { get ; set; }
    }
}

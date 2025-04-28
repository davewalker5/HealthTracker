using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Measurements
{
    [ExcludeFromCodeCoverage]
    public class BloodPressureMeasurement : MeasurementBase
    {
        public int Systolic { get ; set; }
        public int Diastolic { get; set; }

        [NotMapped]
        public string Assessment { get; set; }
    }
}

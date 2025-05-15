using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Measurements
{
    [ExcludeFromCodeCoverage]
    public class BloodPressureMeasurement : MeasurementBase
    {
        [DisplayName("Systolic")]
        [Range(1, int.MaxValue, ErrorMessage = "{0} must be >= {1}")]
        public int Systolic { get ; set; }

        [DisplayName("Diastolic")]
        [Range(1, int.MaxValue, ErrorMessage = "{0} must be >= {1}")]
        public int Diastolic { get; set; }

        [NotMapped]
        public string Assessment { get; set; }
    }
}

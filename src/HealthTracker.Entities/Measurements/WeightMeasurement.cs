using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Measurements
{
    [ExcludeFromCodeCoverage]
    public class WeightMeasurement : MeasurementBase
    {
        [DisplayName("Weight")]
        [Range(1.0, double.MaxValue, ErrorMessage = "{0} must be >= {1}")]
        public decimal Weight { get ; set; }

        [NotMapped]
        public decimal BMI { get; set; }

        [NotMapped]
        public string BMIAssessment { get; set; }

        [NotMapped]
        public decimal BMR { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Measurements
{
    [ExcludeFromCodeCoverage]
    public class WeightMeasurement : MeasurementBase
    {
        public decimal Weight { get ; set; }

        [NotMapped]
        public decimal BMI { get; set; }

        [NotMapped]
        public string BMIAssessment { get; set; }

        [NotMapped]
        public decimal BMR { get; set; }
    }
}

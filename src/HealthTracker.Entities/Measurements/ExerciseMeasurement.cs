using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Measurements
{
    [ExcludeFromCodeCoverage]
    public class ExerciseMeasurement : MeasurementBase
    {
        [DisplayName("Activity")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select an activity")]
        [ForeignKey(nameof(ActivityType))]
        public int ActivityTypeId { get; set; }

        [DisplayName("Duration")]
        public int Duration { get; set; }

        public decimal? Distance { get; set; }

        [DisplayName("Calories")]
        [Range(1, int.MaxValue, ErrorMessage = "{0} must be >= {1}")]
        public int Calories { get; set; }

        [DisplayName("Minimum Heart Rate")]
        [Range(1, int.MaxValue, ErrorMessage = "{0} must be >= {1}")]
        public int MinimumHeartRate { get; set; }

        [DisplayName("Maximum Heart Rate")]
        [Range(1, int.MaxValue, ErrorMessage = "{0} must be >= {1}")]
        public int MaximumHeartRate { get; set; }

        [NotMapped]
        public string ActivityType { get; set; }

        [NotMapped]
        public string FormattedDuration { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Measurements
{
    [ExcludeFromCodeCoverage]
    public class ExerciseMeasurement : MeasurementBase
    {
        public int ActivityTypeId { get; set; }
        public int Duration { get; set; }
        public decimal? Distance { get; set; }
        public int Calories { get; set; }
        public int MinimumHeartRate { get; set; }
        public int MaximumHeartRate { get; set; }

        [NotMapped]
        public string ActivityType { get; set; }

        [NotMapped]
        public string FormattedDuration { get; set; }
    }
}

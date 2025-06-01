using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Measurements
{
    [ExcludeFromCodeCoverage]
    public class ExerciseSummary
    {
        public int? PersonId { get; set; }
        public string PersonName { get; set; }
        public DateTime? From { get; set; } = null;
        public DateTime? To { get; set; } = null;
        public int? ActivityTypeId { get; set; } = null;
        public string ActivityDescription { get; set; }
        public int Count { get; set; } = 0;
        public int TotalDuration { get; set; } = 0;
        public string FormattedDuration { get; set; }
        public decimal TotalDistance { get; set; } = 0;
        public int TotalCalories { get; set; } = 0;
        public int MinimumHeartRate { get; set; } = 0;
        public int MaximumHeartRate { get; set; } = 0;
    }
}
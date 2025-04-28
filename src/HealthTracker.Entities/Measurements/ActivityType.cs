using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Measurements
{
    [ExcludeFromCodeCoverage]
    public class ActivityType
    {
        [Key]
        public int Id { get ; set; }
        public string Description { get; set; }
    }
}
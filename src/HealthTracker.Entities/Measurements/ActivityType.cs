using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Measurements
{
    [ExcludeFromCodeCoverage]
    public class ActivityType : HealthTrackerEntityBase
    {
        [Key]
        public int Id { get ; set; }

        [DisplayName("Description")]
        [Required(ErrorMessage = "You must provide a description")]
        public string Description { get; set; }

        [DisplayName("Distance Based")]
        public bool DistanceBased { get; set; }
    }
}
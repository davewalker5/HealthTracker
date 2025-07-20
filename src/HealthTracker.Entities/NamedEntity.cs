using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities
{
    [ExcludeFromCodeCoverage]
    public abstract class NamedEntity : HealthTrackerEntityBase
    {
        [Key]
        public int Id { get ; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "You must provide a name")]
        public string Name { get; set; }
    }
}
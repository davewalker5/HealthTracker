using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Medications
{
    [ExcludeFromCodeCoverage]
    public class Medication
    {
        [Key]
        public int Id { get ; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "You must provide a name")]
        public string Name { get; set; }
    }
}
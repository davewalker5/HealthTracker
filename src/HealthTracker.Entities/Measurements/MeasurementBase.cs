using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using HealthTracker.Entities.Identity;

namespace HealthTracker.Entities.Measurements
{
    [ExcludeFromCodeCoverage]
    public class MeasurementBase
    {
        [Key]
        public int Id { get ; set; }

        [DisplayName("Person")]
        [Required(ErrorMessage = "You must select a person")]
        [ForeignKey(nameof(Person))]
        public int PersonId { get; set; }

        [DisplayName("Date")]
        [Required(ErrorMessage = "You must provide a measurement date")]
        public DateTime Date { get; set; }
    }
}

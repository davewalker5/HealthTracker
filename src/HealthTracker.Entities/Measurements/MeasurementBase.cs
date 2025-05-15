using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Measurements
{
    [ExcludeFromCodeCoverage]
    public class MeasurementBase
    {
        [Key]
        public int Id { get ; set; }

        [DisplayName("Person")]
        [Required(ErrorMessage = "You must select a person")]
        public int PersonId { get; set; }

        [DisplayName("Date")]
        [Required(ErrorMessage = "You must provide a measurement date")]
        public DateTime Date { get; set; }
    }
}

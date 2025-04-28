using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Measurements
{
    [ExcludeFromCodeCoverage]
    public class MeasurementBase
    {
        [Key]
        public int Id { get ; set; }
        public int PersonId { get; set; }
        public DateTime Date { get; set; }
    }
}

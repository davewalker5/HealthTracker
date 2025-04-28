using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Medications
{
    [ExcludeFromCodeCoverage]
    public class Medication
    {
        [Key]
        public int Id { get ; set; }
        public string Name { get; set; }
    }
}
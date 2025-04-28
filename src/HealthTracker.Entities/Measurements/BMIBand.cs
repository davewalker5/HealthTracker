using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Measurements
{
    [ExcludeFromCodeCoverage]
    public class BMIBand
    {
        [Key]
        public int Id { get ; set; }
        public string Name { get; set; }
        public decimal MinimumBMI { get; set; }
        public decimal MaximumBMI { get; set; }
        public int Order { get; set; }
    }
}
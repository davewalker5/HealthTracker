using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Measurements
{
    [ExcludeFromCodeCoverage]
    public class BloodOxygenSaturationBand
    {
        [Key]
        public int Id { get ; set; }
        public string Name { get; set; }
        public decimal MinimumSPO2 { get; set; }
        public decimal MaximumSPO2 { get; set; }
        public int MinimumAge { get; set; }
        public int MaximumAge { get; set; }
    }
}

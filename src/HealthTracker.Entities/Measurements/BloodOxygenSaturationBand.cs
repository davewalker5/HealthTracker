using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Measurements
{
    [ExcludeFromCodeCoverage]
    public class BloodOxygenSaturationBand : NamedEntity
    {
        public decimal MinimumSPO2 { get; set; }
        public decimal MaximumSPO2 { get; set; }
        public int MinimumAge { get; set; }
        public int MaximumAge { get; set; }
    }
}

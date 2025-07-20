using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Measurements
{
    [ExcludeFromCodeCoverage]
    public class BloodPressureBand : NamedEntity
    {
        public int MinimumSystolic { get; set; }
        public int MaximumSystolic { get; set; }
        public int MinimumDiastolic { get; set; }
        public int MaximumDiastolic { get; set; }
        public int Order { get; set; }
        public bool MatchAll { get; set; }
    }
}
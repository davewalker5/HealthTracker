using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Measurements
{
    [ExcludeFromCodeCoverage]
    public class BloodPressureBand
    {
        [Key]
        public int Id { get ; set; }
        public string Name { get; set; }
        public int MinimumSystolic { get; set; }
        public int MaximumSystolic { get; set; }
        public int MinimumDiastolic { get; set; }
        public int MaximumDiastolic { get; set; }
        public int Order { get; set; }
        public bool MatchAll { get; set; }
    }
}
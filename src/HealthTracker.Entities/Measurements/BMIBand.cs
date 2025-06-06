using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Measurements
{
    [ExcludeFromCodeCoverage]
    public class BMIBand : NamedEntity
    {
        public decimal MinimumBMI { get; set; }
        public decimal MaximumBMI { get; set; }
        public int Order { get; set; }
    }
}
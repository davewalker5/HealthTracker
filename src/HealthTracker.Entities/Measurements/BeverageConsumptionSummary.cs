using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Measurements
{
    [ExcludeFromCodeCoverage]
    public class BeverageConsumptionSummary
    {
        public int? PersonId { get; set; }
        public string PersonName { get; set; }
        public DateTime? From { get; set; } = null;
        public DateTime? To { get; set; } = null;
        public int? BeverageId { get; set; }
        public string BeverageName { get; set; }
        public decimal TotalVolume { get; set; }
        public decimal TotalUnits { get; set; }
    }
}

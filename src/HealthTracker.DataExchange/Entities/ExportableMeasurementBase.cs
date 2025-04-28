using HealthTracker.DataExchange.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportableMeasurementBase : ExportableEntityBase
    {
        [Export("Person ID", 1)]
        public int PersonId { get; set; }

        [Export("Name", 2)]
        public string Name { get; set; }

        [Export("Date", 3)]
        public DateTime Date { get; set; }
    }
}

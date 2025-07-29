using HealthTracker.DataExchange.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportableShoppingListItem : ExportableEntityBase
    {
        [Export("Item", 1)]
        public string Item { get; set; }

        [Export("Portion", 2)]
        public decimal Portion { get; set; }

        [Export("Quantity", 3)]
        public int Quantity { get; set; }
    }
}
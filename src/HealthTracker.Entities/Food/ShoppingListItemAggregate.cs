using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Food
{
    [ExcludeFromCodeCoverage]
    public class ShoppingListItemAggregate : HealthTrackerEntityBase
    {
        public int FoodItemId { get; set; }
        public string Item { get; set; }
        public Dictionary<decimal, int> Quantities { get; set; }
    }
}
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Entities.Food
{
    [ExcludeFromCodeCoverage]
    public class ShoppingListItem : HealthTrackerEntityBase
    {
        public int FoodItemId { get; set; }
        public decimal Portion { get; set; }
        public int Quantity { get; set; }
        public string Item { get; set; }
    }
}
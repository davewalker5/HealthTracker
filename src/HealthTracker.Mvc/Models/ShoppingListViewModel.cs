using HealthTracker.Entities;
using HealthTracker.Entities.Food;

namespace HealthTracker.Mvc.Models
{
    public class ShoppingListViewModel : HealthTrackerEntityBase
    {
        public int PersonId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string PersonName { get; set; }
        public string Message { get; set; }
        public IEnumerable<ShoppingListItem> Items { get; set; }
    }
}
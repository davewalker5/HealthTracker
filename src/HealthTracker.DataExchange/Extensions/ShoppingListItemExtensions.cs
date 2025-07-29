using HealthTracker.Entities.Food;
using HealthTracker.DataExchange.Entities;

namespace HealthTracker.DataExchange.Extensions
{
    public static class ShoppingListItemExtensions
    {
        /// <summary>
        /// Return an exportable shopping list item from a shopping list item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static ExportableShoppingListItem ToExportable(this ShoppingListItem item)
            => new()
            {
                Quantity = item.Quantity,
                Portion = item.Portion,
                Item = item.Item
            };

        /// <summary>
        /// Return a collection of exportable shopping list items from a collection of shopping list items
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static IEnumerable<ExportableShoppingListItem> ToExportable(this IEnumerable<ShoppingListItem> items)
        {
            var exportable = new List<ExportableShoppingListItem>();

            foreach (var item in items)
            {
                exportable.Add(item.ToExportable());
            }

            return exportable;
        }
    }
}

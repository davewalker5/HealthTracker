using HealthTracker.Client.Interfaces;
using HealthTracker.Mvc.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Helpers
{
    public class FoodItemListGenerator : IFoodItemListGenerator
    {
        private readonly IFoodItemClient _client;
        private readonly ILogger<FilterGenerator> _logger;

        public FoodItemListGenerator(IFoodItemClient client, ILogger<FilterGenerator> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Generate a list of select list items for food items belonging to the specified category
        /// </summary>
        /// <param name="foodCategoryId"></param>
        /// <returns></returns>
        public async Task<IList<SelectListItem>> Create(int foodCategoryId)
        {
            var list = new List<SelectListItem>();

            // Load the list of activity types
            var items = await _client.ListAsync(foodCategoryId, 1, int.MaxValue);
            var plural = items.Count == 1 ? "" : "s";
            _logger.LogDebug($"{items.Count} food item{plural} loaded via the service");

            // Create a list of select list items from the list of food items. Add an empty entry if there
            // is more than one. If not, the list will only contain that one food item which will be the default
            // selection
            if (items.Count != 1)
            {
                list.Add(new SelectListItem() { Text = "", Value = "0" });
            }

            // Now add an entry for each activity type
            foreach (var item in items)
            {
                list.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }

            return list;
        }
    }
}
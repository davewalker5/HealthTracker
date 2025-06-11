using HealthTracker.Client.Interfaces;
using HealthTracker.Mvc.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Helpers
{
    public class FoodCategoryListGenerator : IFoodCategoryListGenerator
    {
        private readonly IFoodCategoryClient _client;
        private readonly ILogger<FilterGenerator> _logger;

        public FoodCategoryListGenerator(IFoodCategoryClient client, ILogger<FilterGenerator> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Generate a list of select list items for food categories
        /// </summary>
        /// <returns></returns>
        public async Task<IList<SelectListItem>> Create()
        {
            var list = new List<SelectListItem>();

            // Load the list of food categories
            var categories = await _client.ListAsync(1, int.MaxValue);
            var plural = categories.Count == 1 ? "category" : "categories";
            _logger.LogDebug($"{categories.Count} food {plural} loaded via the service");

            // Create a list of select list items from the list of categories. Add an empty entry if there
            // is more than one. If not, the list will only contain that one category which will be the default
            // selection
            if (categories.Count != 1)
            {
                list.Add(new SelectListItem() { Text = "", Value = "0" });
            }

            // Now add an entry for each activity type
            foreach (var category in categories)
            {
                list.Add(new SelectListItem() { Text = category.Name, Value = category.Id.ToString() });
            }

            return list;
        }
    }
}
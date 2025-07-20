using HealthTracker.Client.Interfaces;
using HealthTracker.Mvc.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Helpers
{
    public class MealListGenerator : IMealListGenerator
    {
        private readonly IMealClient _client;
        private readonly ILogger<MealListGenerator> _logger;

        public MealListGenerator(IMealClient client, ILogger<MealListGenerator> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Generate a list of select list items for meals
        /// </summary>
        /// <param name="foodSourceId"></param>
        /// <returns></returns>
        public async Task<IList<SelectListItem>> Create(int foodSourceId)
        {
            var list = new List<SelectListItem>();

            // Load the list of meals
            var meals = await _client.ListAsync(foodSourceId, 1, int.MaxValue);
            var plural = meals.Count == 1 ? "" : "s";
            _logger.LogDebug($"{meals.Count} meal{plural} loaded via the service");

            // Create a list of select list items from the list of meals. Add an empty entry if there
            // is more than one. If not, the list will only contain that one meal which will be the default
            // selection
            if (meals.Count != 1)
            {
                list.Add(new SelectListItem() { Text = "", Value = "0" });
            }

            // Now add an entry for each meal
            foreach (var meal in meals)
            {
                list.Add(new SelectListItem() { Text = meal.Name, Value = meal.Id.ToString() });
            }

            return list;
        }
    }
}
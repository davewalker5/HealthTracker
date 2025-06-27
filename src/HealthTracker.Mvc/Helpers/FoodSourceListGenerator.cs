using HealthTracker.Client.Interfaces;
using HealthTracker.Mvc.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Helpers
{
    public class FoodSourceListGenerator : IFoodSourceListGenerator
    {
        private readonly IFoodSourceClient _client;
        private readonly ILogger<FilterGenerator> _logger;

        public FoodSourceListGenerator(IFoodSourceClient client, ILogger<FilterGenerator> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Generate a list of select list items for food sources
        /// </summary>
        /// <returns></returns>
        public async Task<IList<SelectListItem>> Create()
        {
            var list = new List<SelectListItem>();

            // Load the list of food sources
            var sources = await _client.ListAsync(1, int.MaxValue);
            var plural = sources.Count == 1 ? "" : "s";
            _logger.LogDebug($"{sources.Count} food source{plural} loaded via the service");

            // Create a list of select list items from the list of sources. Add an empty entry if there
            // is more than one. If not, the list will only contain that one source which will be the default
            // selection
            if (sources.Count != 1)
            {
                list.Add(new SelectListItem() { Text = "", Value = "0" });
            }

            // Now add an entry for each activity type
            foreach (var source in sources)
            {
                list.Add(new SelectListItem() { Text = source.Name, Value = source.Id.ToString() });
            }

            return list;
        }
    }
}
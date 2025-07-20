using HealthTracker.Client.Interfaces;
using HealthTracker.Mvc.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Helpers
{
    public class BeverageListGenerator : IBeverageListGenerator
    {
        private readonly IBeverageClient _client;
        private readonly ILogger<FilterGenerator> _logger;

        public BeverageListGenerator(IBeverageClient client, ILogger<FilterGenerator> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Generate a list of select list items for beverages
        /// </summary>
        /// <returns></returns>
        public async Task<IList<SelectListItem>> Create()
        {
            var list = new List<SelectListItem>();

            // Load the list of beverages
            var beverages = await _client.ListAsync(1, int.MaxValue);
            var plural = beverages.Count == 1 ? "" : "s";
            _logger.LogDebug($"{beverages.Count} beverage{plural} loaded via the service");

            // Create a list of select list items from the list of beverages. Add an empty entry if there
            // is more than one. If not, the list will only contain that one beverage which will be the default
            // selection
            if (beverages.Count != 1)
            {
                list.Add(new SelectListItem() { Text = "", Value = "0" });
            }

            // Now add an entry for each beverage
            foreach (var beverage in beverages)
            {
                list.Add(new SelectListItem() { Text = beverage.Name, Value = beverage.Id.ToString() });
            }

            return list;
        }
    }
}
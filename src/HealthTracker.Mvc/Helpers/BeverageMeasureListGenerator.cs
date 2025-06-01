using HealthTracker.Client.Interfaces;
using HealthTracker.Mvc.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Helpers
{
    public class BeverageMeasureListGenerator : IBeverageMeasureListGenerator
    {
        private readonly IBeverageMeasureClient _client;
        private readonly ILogger<FilterGenerator> _logger;

        public BeverageMeasureListGenerator(IBeverageMeasureClient client, ILogger<FilterGenerator> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Generate a list of select list items for beverage measures
        /// </summary>
        /// <returns></returns>
        public async Task<IList<SelectListItem>> Create()
        {
            var list = new List<SelectListItem>();

            // Load the list of beverage measures
            var measures = await _client.ListAsync(1, int.MaxValue);
            var plural = measures.Count == 1 ? "" : "s";
            _logger.LogDebug($"{measures.Count} beverage measure{plural} loaded via the service");

            // Create a list of select list items from the list of beverage measures, starting with the default
            // (empty) selection
            list.Add(new SelectListItem() { Text = "", Value = "0" });

            // Now add an entry for each beverage measure
            foreach (var measure in measures)
            {
                list.Add(new SelectListItem() { Text = measure.Name, Value = measure.Volume.ToString() });
            }

            return list;
        }
    }
}
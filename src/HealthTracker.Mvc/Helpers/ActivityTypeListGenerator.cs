using HealthTracker.Client.Interfaces;
using HealthTracker.Mvc.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthTracker.Mvc.Helpers
{
    public class ActivityTypeListGenerator : IActivityTypeListGenerator
    {
        private readonly IActivityTypeClient _client;
        private readonly ILogger<FilterGenerator> _logger;

        public ActivityTypeListGenerator(IActivityTypeClient client, ILogger<FilterGenerator> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Generate a list of select list items for activity types
        /// </summary>
        /// <returns></returns>
        public async Task<IList<SelectListItem>> Create()
        {
            var list = new List<SelectListItem>();

            // Load the list of activity types
            var activityTypes = await _client.ListActivityTypesAsync(1, int.MaxValue);
            var plural = activityTypes.Count == 1 ? "" : "s";
            _logger.LogDebug($"{activityTypes.Count} activity type{plural} loaded via the service");

            // Create a list of select list items from the list of activity types. Add an empty entry if there
            // is more than one. If not, the list will only contain that one activity type which will be the default
            // selection
            if (activityTypes.Count != 1)
            {
                list.Add(new SelectListItem() { Text = "", Value = "0" });
            }

            // Now add an entry for each person
            foreach (var activityType in activityTypes)
            {
                list.Add(new SelectListItem() { Text = activityType.Description, Value = activityType.Id.ToString() });
            }

            return list;
        }
    }
}
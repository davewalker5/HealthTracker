using HealthTracker.Api.Entities;
using HealthTracker.Api.Interfaces;
using HealthTracker.Configuration.Entities;
using Microsoft.Extensions.Options;
using HealthTracker.Entities.Interfaces;

namespace HealthTracker.Api.Services
{
    public class PurgePlannedMealsService : BackgroundQueueProcessor<PurgePlannedMealsWorkItem>
    {
        private readonly HealthTrackerApplicationSettings _settings;

        public PurgePlannedMealsService(
            ILogger<BackgroundQueueProcessor<PurgePlannedMealsWorkItem>> logger,
            IBackgroundQueue<PurgePlannedMealsWorkItem> queue,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<HealthTrackerApplicationSettings> settings)
            : base(logger, queue, serviceScopeFactory)
        {
            _settings = settings.Value;
        }

        /// <summary>
        /// Perform the purge
        /// </summary>
        /// <param name="item"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        protected override async Task ProcessWorkItemAsync(PurgePlannedMealsWorkItem item, IHealthTrackerFactory factory)
        {
            var reportCutoff = item.Cutoff?.ToString() ?? "today";
            MessageLogger.LogInformation($"Purging planned meals for person with ID {item.PersonId} scheduled before {reportCutoff}");
            await factory.PlannedMeals.PurgeAsync(item.PersonId, item.Cutoff);
            MessageLogger.LogInformation($"Purge of planned meals completed");
        }
    }
}
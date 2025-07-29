using HealthTracker.Api.Entities;
using HealthTracker.Api.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.DataExchange.Export;
using Microsoft.Extensions.Options;
using HealthTracker.Entities.Interfaces;

namespace HealthTracker.Api.Services
{
    public class MealConsumptionCalculationService : BackgroundQueueProcessor<RecalculateMealConsumptionWorkItem>
    {
        private readonly HealthTrackerApplicationSettings _settings;

        public MealConsumptionCalculationService(
            ILogger<BackgroundQueueProcessor<RecalculateMealConsumptionWorkItem>> logger,
            IBackgroundQueue<RecalculateMealConsumptionWorkItem> queue,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<HealthTrackerApplicationSettings> settings)
            : base(logger, queue, serviceScopeFactory)
        {
            _settings = settings.Value;
        }

        /// <summary>
        /// Perform the recalculation
        /// </summary>
        /// <param name="item"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        protected override async Task ProcessWorkItemAsync(RecalculateMealConsumptionWorkItem item, IHealthTrackerFactory factory)
        {
            MessageLogger.LogInformation($"Recalculating nutritional values for all meal consumption records");
            await factory.MealConsumptionMeasurements.UpdateAllNutritionalValues();
            MessageLogger.LogInformation($"Meal consumption nutritional value recalculation completed");
        }
    }
}
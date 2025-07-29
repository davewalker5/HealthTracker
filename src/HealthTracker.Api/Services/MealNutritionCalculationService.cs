using HealthTracker.Api.Entities;
using HealthTracker.Api.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.DataExchange.Export;
using Microsoft.Extensions.Options;
using HealthTracker.Entities.Interfaces;

namespace HealthTracker.Api.Services
{
    public class MealNutritionCalculationService : BackgroundQueueProcessor<RecalculateMealNutritionWorkItem>
    {
        private readonly HealthTrackerApplicationSettings _settings;

        public MealNutritionCalculationService(
            ILogger<BackgroundQueueProcessor<RecalculateMealNutritionWorkItem>> logger,
            IBackgroundQueue<RecalculateMealNutritionWorkItem> queue,
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
        protected override async Task ProcessWorkItemAsync(RecalculateMealNutritionWorkItem item, IHealthTrackerFactory factory)
        {
            MessageLogger.LogInformation($"Recalculating nutritional values for all meals");
            await factory.Meals.UpdateAllNutritionalValues();
            MessageLogger.LogInformation($"Meal nutritional value recalculation completed");
        }
    }
}
using HealthTracker.Api.Entities;
using HealthTracker.Api.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.DataExchange.Export;
using Microsoft.Extensions.Options;
using HealthTracker.Entities.Interfaces;

namespace HealthTracker.Api.Services
{
    public class MealFoodItemExportService : BackgroundQueueProcessor<MealFoodItemExportWorkItem>
    {
        private readonly HealthTrackerApplicationSettings _settings;

        public MealFoodItemExportService(
            ILogger<BackgroundQueueProcessor<MealFoodItemExportWorkItem>> logger,
            IBackgroundQueue<MealFoodItemExportWorkItem> queue,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<HealthTrackerApplicationSettings> settings)
            : base(logger, queue, serviceScopeFactory)
        {
            _settings = settings.Value;
        }

        /// <summary>
        /// Export the data
        /// </summary>
        /// <param name="item"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        protected override async Task ProcessWorkItemAsync(MealFoodItemExportWorkItem item, IHealthTrackerFactory factory)
        {
            // Get the list of meal/food item relationships to export
            MessageLogger.LogInformation("Retrieving meal/food item relationships for export");
            var relationships = await factory.MealFoodItems.ListAsync(x => true);

            // Get the full path to the export file
            var filePath = Path.Combine(_settings.ExportPath, item.FileName);

            // Export the meal/food item relationships
            MessageLogger.LogInformation($"Exporting {relationships.Count} meal/food item relationships to {filePath}");
            var exporter = new MealFoodItemExporter(factory);
            await exporter.ExportAsync(relationships, filePath);
            MessageLogger.LogInformation("Meal/food item relationship export completed");
        }
    }
}
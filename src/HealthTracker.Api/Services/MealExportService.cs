using HealthTracker.Api.Entities;
using HealthTracker.Api.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.DataExchange.Export;
using Microsoft.Extensions.Options;
using HealthTracker.Entities.Interfaces;

namespace HealthTracker.Api.Services
{
    public class MealExportService : BackgroundQueueProcessor<MealExportWorkItem>
    {
        private readonly HealthTrackerApplicationSettings _settings;

        public MealExportService(
            ILogger<BackgroundQueueProcessor<MealExportWorkItem>> logger,
            IBackgroundQueue<MealExportWorkItem> queue,
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
        protected override async Task ProcessWorkItemAsync(MealExportWorkItem item, IHealthTrackerFactory factory)
        {
            // Get the list of meals to export
            MessageLogger.LogInformation("Retrieving meals for export");
            var meals = await factory.Meals.ListAsync(x => true, 1, int.MaxValue);

            // Get the full path to the export file
            var filePath = Path.Combine(_settings.ExportPath, item.FileName);

            // Export the meals
            MessageLogger.LogInformation($"Exporting {meals.Count} meals to {filePath}");
            var exporter = new MealExporter(factory);
            await exporter.ExportAsync(meals, filePath);
            MessageLogger.LogInformation("Meal export completed");
        }
    }
}
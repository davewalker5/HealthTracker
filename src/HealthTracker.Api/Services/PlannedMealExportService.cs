using HealthTracker.Api.Entities;
using HealthTracker.Api.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.DataExchange.Export;
using Microsoft.Extensions.Options;
using HealthTracker.Entities.Interfaces;

namespace HealthTracker.Api.Services
{
    public class PlannedMealExportService : BackgroundQueueProcessor<PlannedMealExportWorkItem>
    {
        private readonly HealthTrackerApplicationSettings _settings;

        public PlannedMealExportService(
            ILogger<BackgroundQueueProcessor<PlannedMealExportWorkItem>> logger,
            IBackgroundQueue<PlannedMealExportWorkItem> queue,
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
        protected override async Task ProcessWorkItemAsync(PlannedMealExportWorkItem item, IHealthTrackerFactory factory)
        {
            // Get the list of meals to export
            MessageLogger.LogInformation("Retrieving planned meals for export");
            var meals = await factory.PlannedMeals.ListAsync(x => true, 1, int.MaxValue);

            // Get the full path to the export file
            var filePath = Path.Combine(_settings.ExportPath, item.FileName);

            // Export the meals
            MessageLogger.LogInformation($"Exporting {meals.Count} planned meals to {filePath}");
            var exporter = new PlannedMealExporter(factory);
            await exporter.ExportAsync(meals, filePath);
            MessageLogger.LogInformation("Planned meal export completed");
        }
    }
}
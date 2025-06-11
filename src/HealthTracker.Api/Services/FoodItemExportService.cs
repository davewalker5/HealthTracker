using HealthTracker.Api.Entities;
using HealthTracker.Api.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.DataExchange.Export;
using Microsoft.Extensions.Options;
using HealthTracker.Entities.Interfaces;

namespace HealthTracker.Api.Services
{
    public class FoodItemExportService : BackgroundQueueProcessor<FoodItemExportWorkItem>
    {
        private readonly HealthTrackerApplicationSettings _settings;

        public FoodItemExportService(
            ILogger<BackgroundQueueProcessor<FoodItemExportWorkItem>> logger,
            IBackgroundQueue<FoodItemExportWorkItem> queue,
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
        protected override async Task ProcessWorkItemAsync(FoodItemExportWorkItem item, IHealthTrackerFactory factory)
        {
            // Get the list of items to export
            MessageLogger.LogInformation("Retrieving food items for export");
            var items = await factory.FoodItems.ListAsync(x => true, 1, int.MaxValue);

            // Get the full path to the export file
            var filePath = Path.Combine(_settings.ExportPath, item.FileName);

            // Export the items
            MessageLogger.LogInformation($"Exporting {items.Count} food items to {filePath}");
            var exporter = new FoodItemExporter(factory);
            await exporter.ExportAsync(items, filePath);
            MessageLogger.LogInformation("Food item export completed");
        }
    }
}
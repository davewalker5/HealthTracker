using HealthTracker.Api.Entities;
using HealthTracker.Api.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.DataExchange.Export;
using Microsoft.Extensions.Options;
using HealthTracker.Entities.Interfaces;

namespace HealthTracker.Api.Services
{
    public class ShoppingListExportService : BackgroundQueueProcessor<ShoppingListExportWorkItem>
    {
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        private readonly HealthTrackerApplicationSettings _settings;

        public ShoppingListExportService(
            ILogger<BackgroundQueueProcessor<ShoppingListExportWorkItem>> logger,
            IBackgroundQueue<ShoppingListExportWorkItem> queue,
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
        protected override async Task ProcessWorkItemAsync(ShoppingListExportWorkItem item, IHealthTrackerFactory factory)
        {
            // Get the full path to the export file
            var filePath = Path.Combine(_settings.ExportPath, item.FileName);

            // Export the shopping list
            MessageLogger.LogInformation($"Exporting shopping list for the person with ID {item.PersonId} for the date range {item.From} to {item.To} to {filePath}");
            var exporter = new ShoppingListItemExporter(factory);
            await exporter.ExportAsync(item.PersonId, item.From, item.To, filePath);
            MessageLogger.LogInformation("Shopping list export completed");
        }
    }
}
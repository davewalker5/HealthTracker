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
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        private readonly HealthTrackerApplicationSettings _settings;
        private readonly IBackgroundQueue<ShoppingListExportWorkItem> _shoppingListQueue;

        public PlannedMealExportService(
            ILogger<BackgroundQueueProcessor<PlannedMealExportWorkItem>> logger,
            IBackgroundQueue<PlannedMealExportWorkItem> plannedMealQueue,
            IBackgroundQueue<ShoppingListExportWorkItem> shoppingListQueue,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<HealthTrackerApplicationSettings> settings)
            : base(logger, plannedMealQueue, serviceScopeFactory)
        {
            _settings = settings.Value;
            _shoppingListQueue = shoppingListQueue;
        }

        /// <summary>
        /// Export the data
        /// </summary>
        /// <param name="item"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        protected override async Task ProcessWorkItemAsync(PlannedMealExportWorkItem item, IHealthTrackerFactory factory)
        {
            // Get the full path to the export file
            var filePath = Path.Combine(_settings.ExportPath, item.FileName);

            // Export the planned meals
            MessageLogger.LogInformation($"Exporting planned meals for the person with ID {item.PersonId} for the date range {item.From} to {item.To} to {filePath}");
            var exporter = new PlannedMealExporter(factory);
            await exporter.ExportAsync(item.PersonId, item.From, item.To, filePath);
            MessageLogger.LogInformation("Planned meal export completed");

            // When scheduled meals are exported, we also enqueue a request to export the shopping list
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var extension = Path.GetExtension(filePath);

            var relationshipExportItem = new ShoppingListExportWorkItem()
            {
                JobName = "Shopping List Export",
                PersonId = item.PersonId,
                From = item.From,
                To = item.To,
                FileName = $"{fileName}-ShoppingList{extension}"
            };

            _shoppingListQueue.Enqueue(relationshipExportItem);
        }
    }
}
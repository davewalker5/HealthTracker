using HealthTracker.Api.Entities;
using HealthTracker.Api.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.DataExchange.Import;
using Microsoft.Extensions.Options;
using HealthTracker.Entities.Interfaces;
using HealthTracker.DataExchange.Entities;

namespace HealthTracker.Api.Services
{
    public class MealConsumptionMeasurementImportService : BackgroundQueueProcessor<MealConsumptionMeasurementImportWorkItem>
    {
        private readonly HealthTrackerApplicationSettings _settings;

        public MealConsumptionMeasurementImportService(
            ILogger<BackgroundQueueProcessor<MealConsumptionMeasurementImportWorkItem>> logger,
            IBackgroundQueue<MealConsumptionMeasurementImportWorkItem> queue,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<HealthTrackerApplicationSettings> settings)
            : base(logger, queue, serviceScopeFactory)
        {
            _settings = settings.Value;
        }

        /// <summary>
        /// Import meal consumption measurements from the data specified in the work item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        protected override async Task ProcessWorkItemAsync(MealConsumptionMeasurementImportWorkItem item, IHealthTrackerFactory factory)
        {
            MessageLogger.LogInformation("Meal consumption measurement import started");
            var records = item.Content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            var count = records.Length - 1;
            if (count > 0)
            {
                var messageEnding = (count > 1) ? "s" : "";
                MessageLogger.LogInformation($"Importing {records.Count() - 1} measurement{messageEnding}");
                var importer = new MealConsumptionMeasurementImporter(factory, ExportableMealConsumptionMeasurement.CsvRecordPattern);
                await importer.ImportAsync(records);
            }
            else
            {
                MessageLogger.LogWarning("No records found to import");
            }

            MessageLogger.LogInformation("Meal consumption measurement import completed");
        }
    }
}
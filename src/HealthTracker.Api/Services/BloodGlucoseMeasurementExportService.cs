using HealthTracker.Api.Entities;
using HealthTracker.Api.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.DataExchange.Export;
using Microsoft.Extensions.Options;
using HealthTracker.Entities.Interfaces;

namespace HealthTracker.Api.Services
{
    public class  BloodGlucoseMeasurementExportService : BackgroundQueueProcessor< BloodGlucoseMeasurementExportWorkItem>
    {
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        private readonly HealthTrackerApplicationSettings _settings;

        public  BloodGlucoseMeasurementExportService(
            ILogger<BackgroundQueueProcessor< BloodGlucoseMeasurementExportWorkItem>> logger,
            IBackgroundQueue< BloodGlucoseMeasurementExportWorkItem> queue,
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
        protected override async Task ProcessWorkItemAsync( BloodGlucoseMeasurementExportWorkItem item, IHealthTrackerFactory factory)
        {
            // Get the full path to the export file
            var filePath = Path.Combine(_settings.ExportPath, item.FileName);

            // Export the measurements
            LogExportMessages(item, filePath);
            var exporter = new  BloodGlucoseMeasurementExporter(factory);
            await exporter.ExportAsync(item.PersonId, item.From, item.To, filePath);
            MessageLogger.LogInformation("Blood glucose measurement export completed");
        }

        /// <summary>
        /// Log the export messages, indicating the date range for export
        /// </summary>
        /// <param name="item"></param>
        /// <param name="filePath"></param>
        private void LogExportMessages( BloodGlucoseMeasurementExportWorkItem item, string filePath)
        {
            MessageLogger.LogInformation($"Exporting blood glucose measurements for the person with ID {item.PersonId} to {filePath}");
            if ((item.From != null) && (item.To != null))
            {
                MessageLogger.LogInformation($"Exporting measurements in the date range {item.From.Value.ToString(DateTimeFormat)} to {item.To.Value.ToString(DateTimeFormat)}");
            }
            else if (item.From != null)
            {
                MessageLogger.LogInformation($"Exporting measurements logged from {item.From.Value.ToString(DateTimeFormat)}");
            }
            else if (item.To != null)
            {
                MessageLogger.LogInformation($"Exporting measurements logged up to {item.To.Value.ToString(DateTimeFormat)}");
            }
        }
    }
}
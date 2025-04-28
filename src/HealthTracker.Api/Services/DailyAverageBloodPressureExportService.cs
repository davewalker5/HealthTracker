using HealthTracker.Api.Entities;
using HealthTracker.Api.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.DataExchange.Export;
using Microsoft.Extensions.Options;
using HealthTracker.Entities.Interfaces;

namespace HealthTracker.Api.Services
{
    public class DailyAverageBloodPressureExportService : BackgroundQueueProcessor<DailyAverageBloodPressureExportWorkItem>
    {
        private readonly HealthTrackerApplicationSettings _settings;

        public DailyAverageBloodPressureExportService(
            ILogger<BackgroundQueueProcessor<DailyAverageBloodPressureExportWorkItem>> logger,
            IBackgroundQueue<DailyAverageBloodPressureExportWorkItem> queue,
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
        protected override async Task ProcessWorkItemAsync(DailyAverageBloodPressureExportWorkItem item, IHealthTrackerFactory factory)
        {
            // Get the full path to the export file
            var filePath = Path.Combine(_settings.ExportPath, item.FileName);

            // Export the measurements
            MessageLogger.LogInformation($"Exporting daily average blood pressure measurements for the person with ID {item.PersonId} and date range {item.From.ToShortDateString()} to {item.To.ToShortDateString()} to {filePath}");
            var exporter = new BloodPressureMeasurementExporter(factory);
            await exporter.ExportDailyAveragesAsync(item.PersonId, item.From, item.To, filePath);
            MessageLogger.LogInformation("Daily average blood pressure measurement export completed");
        }
    }
}
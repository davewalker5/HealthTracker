using HealthTracker.Api.Entities;
using HealthTracker.Api.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.DataExchange.Export;
using Microsoft.Extensions.Options;
using HealthTracker.Entities.Interfaces;

namespace HealthTracker.Api.Services
{
    public class DailyAverageBloodOxygenSaturationExportService : BackgroundQueueProcessor<DailyAverageBloodOxygenSaturationExportWorkItem>
    {
        private readonly HealthTrackerApplicationSettings _settings;

        public DailyAverageBloodOxygenSaturationExportService(
            ILogger<BackgroundQueueProcessor<DailyAverageBloodOxygenSaturationExportWorkItem>> logger,
            IBackgroundQueue<DailyAverageBloodOxygenSaturationExportWorkItem> queue,
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
        protected override async Task ProcessWorkItemAsync(DailyAverageBloodOxygenSaturationExportWorkItem item, IHealthTrackerFactory factory)
        {
            // Get the full path to the export file
            var filePath = Path.Combine(_settings.ExportPath, item.FileName);

            // Export the measurements
            MessageLogger.LogInformation($"Exporting daily average % SPO2 measurements for the person with ID {item.PersonId} and date range {item.From.ToShortDateString()} to {item.To.ToShortDateString()} to {filePath}");
            var exporter = new BloodOxygenSaturationMeasurementExporter(factory);
            await exporter.ExportDailyAveragesAsync(item.PersonId, item.From, item.To, filePath);
            MessageLogger.LogInformation("Daily average % SPO2 measurement export completed");
        }
    }
}
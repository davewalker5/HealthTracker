using HealthTracker.Api.Entities;
using HealthTracker.Api.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.DataExchange.Import;
using Microsoft.Extensions.Options;
using HealthTracker.Entities.Interfaces;
using HealthTracker.DataExchange.Entities;

namespace HealthTracker.Api.Services
{
    public class OmronBloodPressureImportService : BackgroundQueueProcessor<OmronBloodPressureImportWorkItem>
    {
        private readonly HealthTrackerApplicationSettings _settings;

        public OmronBloodPressureImportService(
            ILogger<BackgroundQueueProcessor<OmronBloodPressureImportWorkItem>> logger,
            IBackgroundQueue<OmronBloodPressureImportWorkItem> queue,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<HealthTrackerApplicationSettings> settings)
            : base(logger, queue, serviceScopeFactory)
        {
            _settings = settings.Value;
        }

        /// <summary>
        /// Import blood pressure measurements from the data specified in the work item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        protected override async Task ProcessWorkItemAsync(OmronBloodPressureImportWorkItem item, IHealthTrackerFactory factory)
        {
            MessageLogger.LogInformation("OMRON blood pressure measurement import started");

            if (item.Content.Length > 0)
            {
                MessageLogger.LogInformation($"Importing measurements");
                var bloodPressureImporter = new BloodPressureMeasurementImporter(factory, ExportableBloodPressureMeasurement.CsvRecordPattern);
                var importer = new OmronConnectBloodPressureImporter(
                    factory,
                    bloodPressureImporter,
                    _settings.OmronDateColumnTitle,
                    _settings.OmronSystolicColumnTitle,
                    _settings.OmronDiastolicColumnTitle);
                await importer.ImportAsync(item.Content, item.PersonId);
            }
            else
            {
                MessageLogger.LogWarning("No records found to import");
            }

            MessageLogger.LogInformation("OMRON blood pressure measurement import completed");
        }
    }
}
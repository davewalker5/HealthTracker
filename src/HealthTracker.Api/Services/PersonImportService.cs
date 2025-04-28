using HealthTracker.Api.Entities;
using HealthTracker.Api.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.DataExchange.Import;
using Microsoft.Extensions.Options;
using HealthTracker.Entities.Interfaces;
using HealthTracker.DataExchange.Entities;

namespace HealthTracker.Api.Services
{
    public class PersonImportService : BackgroundQueueProcessor<PersonImportWorkItem>
    {
        private readonly HealthTrackerApplicationSettings _settings;

        public PersonImportService(
            ILogger<BackgroundQueueProcessor<PersonImportWorkItem>> logger,
            IBackgroundQueue<PersonImportWorkItem> queue,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<HealthTrackerApplicationSettings> settings)
            : base(logger, queue, serviceScopeFactory)
        {
            _settings = settings.Value;
        }

        /// <summary>
        /// Import people from the data specified in the work item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        protected override async Task ProcessWorkItemAsync(PersonImportWorkItem item, IHealthTrackerFactory factory)
        {
            MessageLogger.LogInformation("Person import started");
            var records = item.Content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            var count = records.Length - 1;
            if (count > 0)
            {
                var messageEnding = (count > 1) ? "people" : "person";
                MessageLogger.LogInformation($"Importing {records.Count() - 1} {messageEnding}");
                var importer = new PersonImporter(factory, ExportablePerson.CsvRecordPattern);
                await importer.ImportAsync(records);
            }
            else
            {
                MessageLogger.LogWarning("No records found to import");
            }

            MessageLogger.LogInformation("Person import completed");
        }
    }
}
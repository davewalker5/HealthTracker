using HealthTracker.Api.Entities;
using HealthTracker.Api.Interfaces;
using HealthTracker.Configuration.Entities;
using HealthTracker.DataExchange.Export;
using Microsoft.Extensions.Options;
using HealthTracker.Entities.Interfaces;

namespace HealthTracker.Api.Services
{
    public class PersonExportService : BackgroundQueueProcessor<PersonExportWorkItem>
    {
        private readonly HealthTrackerApplicationSettings _settings;

        public PersonExportService(
            ILogger<BackgroundQueueProcessor<PersonExportWorkItem>> logger,
            IBackgroundQueue<PersonExportWorkItem> queue,
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
        protected override async Task ProcessWorkItemAsync(PersonExportWorkItem item, IHealthTrackerFactory factory)
        {
            // Get the list of people to export
            MessageLogger.LogInformation("Retrieving people for export");
            var people = await factory.People.ListAsync(x => true);

            // Get the full path to the export file
            var filePath = Path.Combine(_settings.ExportPath, item.FileName);

            // Export the people
            MessageLogger.LogInformation($"Exporting {people.Count} people to {filePath}");
            var exporter = new PersonExporter();
            exporter.ExportAsync(people, filePath);
            MessageLogger.LogInformation("Person export completed");
        }
    }
}
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Interfaces;
using HealthTracker.DataExchange.Extensions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Food;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DataExchange.Export
{
    [ExcludeFromCodeCoverage]
    public class MealConsumptionMeasurementExporter : IMealConsumptionMeasurementExporter
    {
        private readonly IHealthTrackerFactory _factory;

        public event EventHandler<ExportEventArgs<ExportableMealConsumptionMeasurement>> RecordExport;

        public MealConsumptionMeasurementExporter(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Export the measurements for a person to a CSV file
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="file"></param>
        public async Task ExportAsync(int personId, DateTime? from, DateTime? to, string file)
        {
            var measurements = await _factory.MealConsumptionMeasurements.ListAsync(x =>
                                                (x.PersonId == personId) &&
                                                ((from == null) || (x.Date >= from)) &&
                                                ((to == null) || (x.Date <= to)),
                                                1, int.MaxValue);
            await ExportAsync(measurements, file);
        }

        /// <summary>
        /// Export a collection of alcohol consumption measurements to a CSV file
        /// </summary>
        /// <param name="measurements"></param>
        /// <param name="file"></param>
        public async Task ExportAsync(IEnumerable<MealConsumptionMeasurement> measurements, string file)
        {
            // Get a list of meals so we can map IDs to descriptions and convert the collection to "exportable" equivalents
            // with all properties at the same level
            var people = await _factory.People.ListAsync(x => true, 1, int.MaxValue);
            var meals = await _factory.Meals.ListAsync(x => true, 1, int.MaxValue);
            var exportable = measurements.ToExportable(people, meals);

            // Configure an exporter to export them
            var exporter = new CsvExporter<ExportableMealConsumptionMeasurement>(ExportableEntityBase.TimestampFormat);
            exporter.RecordExport += OnRecordExported;

            // Export the records
            exporter.Export(exportable, file, ',');
        }

        /// <summary>
        /// Handler for alcohol consumption measurement export notifications
        /// </summary>
        /// <param name="_"></param>
        /// <param name="e"></param>
        private void OnRecordExported(object _, ExportEventArgs<ExportableMealConsumptionMeasurement> e)
        {
            RecordExport?.Invoke(this, e);
        }
    }
}

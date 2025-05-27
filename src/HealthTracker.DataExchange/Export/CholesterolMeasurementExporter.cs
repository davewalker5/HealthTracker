using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Interfaces;
using HealthTracker.DataExchange.Extensions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DataExchange.Export
{
    [ExcludeFromCodeCoverage]
    public class CholesterolMeasurementExporter : ICholesterolMeasurementExporter
    {
        private readonly IHealthTrackerFactory _factory;

        public event EventHandler<ExportEventArgs<ExportableCholesterolMeasurement>> RecordExport;

        public CholesterolMeasurementExporter(IHealthTrackerFactory factory)
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
            var measurements = await _factory.CholesterolMeasurements.ListAsync(x =>
                                                (x.PersonId == personId) &&
                                                ((from == null) || (x.Date >= from)) &&
                                                ((to == null) || (x.Date <= to)));
            await ExportAsync(measurements, file);
        }

        /// <summary>
        /// Export a collection of cholesterol measurements to a CSV file
        /// </summary>
        /// <param name="measurements"></param>
        /// <param name="file"></param>
        public async Task ExportAsync(IEnumerable<CholesterolMeasurement> measurements, string file)
        {
            // Convert the collection to "exportable" equivalents with all properties at the same level
            var people = await _factory.People.ListAsync(x => true, 1, int.MaxValue);
            var exportable = measurements.ToExportable(people);

            // Configure an exporter to export them
            var exporter = new CsvExporter<ExportableCholesterolMeasurement>(ExportableEntityBase.TimestampFormat);
            exporter.RecordExport += OnRecordExported;

            // Export the records
            exporter.Export(exportable, file, ',');
        }

        /// <summary>
        /// Handler for cholesterol measurement export notifications
        /// </summary>
        /// <param name="_"></param>
        /// <param name="e"></param>
        private void OnRecordExported(object _, ExportEventArgs<ExportableCholesterolMeasurement> e)
        {
            RecordExport?.Invoke(this, e);
        }
    }
}

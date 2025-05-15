using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Extensions;
using HealthTracker.DataExchange.Interfaces;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DataExchange.Export
{
    [ExcludeFromCodeCoverage]
    public class BloodOxygenSaturationMeasurementExporter : IBloodOxygenSaturationMeasurementExporter
    {
        private readonly IHealthTrackerFactory _factory;

        public event EventHandler<ExportEventArgs<ExportableBloodOxygenSaturationMeasurement>> RecordExport;

        public BloodOxygenSaturationMeasurementExporter(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Export the measurements for a person to a CSV file
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="file"></param>
        public async Task ExportAsync(int personId, DateTime? from, DateTime? to, string file)
        {
            var measurements = await _factory.BloodOxygenSaturationMeasurements.ListAsync(x =>
                                                (x.PersonId == personId) &&
                                                ((from == null) || (x.Date >= from)) &&
                                                ((to == null) || (x.Date <= to)),
                                                1, int.MaxValue);
            await _factory.BloodOxygenSaturationAssessor.Assess(measurements);
            await ExportAsync(measurements, file);
        }

        /// <summary>
        /// Export the measurements for a person to a CSV file
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="file"></param>
        public async Task ExportDailyAveragesAsync(int personId, DateTime from, DateTime to, string file)
        {
            var measurements = await _factory.BloodOxygenSaturationCalculator.DailyAverageAsync(personId, from, to);
            await _factory.BloodOxygenSaturationAssessor.Assess(measurements);
            await ExportAsync(measurements, file);
        }

        /// <summary>
        /// Export a collection of % SPO2 measurements to a CSV file
        /// </summary>
        /// <param name="measurements"></param>
        /// <param name="file"></param>
        public async Task ExportAsync(IEnumerable<BloodOxygenSaturationMeasurement> measurements, string file)
        {
            // Convert the collection to "exportable" equivalents with all properties at the same level
            var people = await _factory.People.ListAsync(x => true, 1, int.MaxValue);
            var exportable = measurements.ToExportable(people);

            // Configure an exporter to export them
            var exporter = new CsvExporter<ExportableBloodOxygenSaturationMeasurement>(ExportableEntityBase.DateTimeFormat);
            exporter.RecordExport += OnRecordExported;

            // Export the records
            exporter.Export(exportable, file, ',');
        }

        /// <summary>
        /// Handler for blood pressure measurement export notifications
        /// </summary>
        /// <param name="_"></param>
        /// <param name="e"></param>
        private void OnRecordExported(object _, ExportEventArgs<ExportableBloodOxygenSaturationMeasurement> e)
        {
            RecordExport?.Invoke(this, e);
        }
    }
}

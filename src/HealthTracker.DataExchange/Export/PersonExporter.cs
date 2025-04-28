using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Interfaces;
using HealthTracker.DataExchange.Extensions;
using HealthTracker.Entities.Identity;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DataExchange.Export
{
    [ExcludeFromCodeCoverage]
    public class PersonExporter : IPersonExporter
    {
        public event EventHandler<ExportEventArgs<ExportablePerson>> RecordExport;

        /// <summary>
        /// Export a collection of people to a CSV file
        /// </summary>
        /// <param name="people"></param>
        /// <param name="file"></param>
        public void ExportAsync(IEnumerable<Person> people, string file)
        {
            // Convert the collection to "exportable" equivalents with all properties at the same level
            IEnumerable<ExportablePerson> exportable = people.ToExportable();

            // Configure an exporter to export them
            var exporter = new CsvExporter<ExportablePerson>(ExportableEntityBase.DateTimeFormat);
            exporter.RecordExport += OnRecordExported;

            // Export the records
            exporter.Export(exportable, file, ',');
        }

        /// <summary>
        /// Handler for person export notifications
        /// </summary>
        /// <param name="_"></param>
        /// <param name="e"></param>
        private void OnRecordExported(object _, ExportEventArgs<ExportablePerson> e)
        {
            RecordExport?.Invoke(this, e);
        }
    }
}

using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Interfaces;
using HealthTracker.DataExchange.Extensions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Food;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DataExchange.Export
{
    [ExcludeFromCodeCoverage]
    public class FoodItemExporter : IFoodItemExporter
    {
        private readonly IHealthTrackerFactory _factory;

        public event EventHandler<ExportEventArgs<ExportableFoodItem>> RecordExport;

        public FoodItemExporter(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Export the items to a CSV file
        /// </summary>
        /// <param name="file"></param>
        public async Task ExportAsync(string file)
        {
            var items = await _factory.FoodItems.ListAsync(x => true, 1, int.MaxValue);
            await ExportAsync(items, file);
        }

        /// <summary>
        /// Export a collection of items to a CSV file
        /// </summary>
        /// <param name="items"></param>
        /// <param name="file"></param>
#pragma warning disable CS1998
        public async Task ExportAsync(IEnumerable<FoodItem> items, string file)
        {
            // Convert to exportable (flattened hierarchy) items
            var exportable = items.ToExportable();

            // Configure an exporter to export them
            var exporter = new CsvExporter<ExportableFoodItem>(null);
            exporter.RecordExport += OnRecordExported;

            // Export the records
            exporter.Export(exportable, file, ',');
        }
#pragma warning restore CS1998

        /// <summary>
        /// Handler for exercise measurement export notifications
        /// </summary>
        /// <param name="_"></param>
        /// <param name="e"></param>
        private void OnRecordExported(object _, ExportEventArgs<ExportableFoodItem> e)
        {
            RecordExport?.Invoke(this, e);
        }
    }
}

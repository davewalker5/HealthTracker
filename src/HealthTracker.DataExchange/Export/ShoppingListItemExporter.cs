using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Interfaces;
using HealthTracker.DataExchange.Extensions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Food;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DataExchange.Export
{
    [ExcludeFromCodeCoverage]
    public class ShoppingListItemExporter : IShoppingListItemExporter
    {
        private readonly IHealthTrackerFactory _factory;

        public event EventHandler<ExportEventArgs<ExportableShoppingListItem>> RecordExport;

        public ShoppingListItemExporter(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Export the planned meals to a CSV file
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="file"></param>
        public async Task ExportAsync(int personId, DateTime from, DateTime to, string file)
        {
            var items = await _factory.PlannedMeals.GetShoppingList(personId, from, to);
            await ExportAsync(items, file);
        }

        /// <summary>
        /// Export a collection of planned meals to a CSV file
        /// </summary>
        /// <param name="items"></param>
        /// <param name="file"></param>
#pragma warning disable CS1998
        public async Task ExportAsync(IEnumerable<ShoppingListItem> items, string file)
        {
            // Get a list of people for ID mapping
            var people = await _factory.People.ListAsync(x => true, 1, int.MaxValue);

            // Convert to exportable (flattened hierarchy) meals
            var exportable = items.ToExportable();

            // Configure an exporter to export them
            var exporter = new CsvExporter<ExportableShoppingListItem>(ExportableEntityBase.TimestampFormat);
            exporter.RecordExport += OnRecordExported;

            // Export the records
            exporter.Export(exportable, file, ',');
        }
#pragma warning restore CS1998

        /// <summary>
        /// Handler for planned meal export notifications
        /// </summary>
        /// <param name="_"></param>
        /// <param name="e"></param>
        private void OnRecordExported(object _, ExportEventArgs<ExportableShoppingListItem> e)
        {
            RecordExport?.Invoke(this, e);
        }
    }
}

using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Interfaces;
using HealthTracker.DataExchange.Extensions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Food;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DataExchange.Export
{
    [ExcludeFromCodeCoverage]
    public class MealExporter : IMealExporter
    {
        private readonly IHealthTrackerFactory _factory;

        public event EventHandler<ExportEventArgs<ExportableMeal>> RecordExport;

        public MealExporter(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Export the meals to a CSV file
        /// </summary>
        /// <param name="file"></param>
        public async Task ExportAsync(string file)
        {
            var meals = await _factory.Meals.ListAsync(x => true, 1, int.MaxValue);
            await ExportAsync(meals, file);
        }

        /// <summary>
        /// Export a collection of meals to a CSV file
        /// </summary>
        /// <param name="meals"></param>
        /// <param name="file"></param>
#pragma warning disable CS1998
        public async Task ExportAsync(IEnumerable<Meal> meals, string file)
        {
            // Convert to exportable (flattened hierarchy) meals
            var exportable = meals.ToExportable();

            // Configure an exporter to export them
            var exporter = new CsvExporter<ExportableMeal>(null);
            exporter.RecordExport += OnRecordExported;

            // Export the records
            exporter.Export(exportable, file, ',');
        }
#pragma warning restore CS1998

        /// <summary>
        /// Handler for meal export notifications
        /// </summary>
        /// <param name="_"></param>
        /// <param name="e"></param>
        private void OnRecordExported(object _, ExportEventArgs<ExportableMeal> e)
        {
            RecordExport?.Invoke(this, e);
        }
    }
}

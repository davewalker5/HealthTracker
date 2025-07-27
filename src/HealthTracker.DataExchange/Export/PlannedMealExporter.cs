using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Interfaces;
using HealthTracker.DataExchange.Extensions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Food;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DataExchange.Export
{
    [ExcludeFromCodeCoverage]
    public class PlannedMealExporter : IPlannedMealExporter
    {
        private readonly IHealthTrackerFactory _factory;

        public event EventHandler<ExportEventArgs<ExportablePlannedMeal>> RecordExport;

        public PlannedMealExporter(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Export the planned meals to a CSV file
        /// </summary>
        /// <param name="file"></param>
        public async Task ExportAsync(string file)
        {
            var meals = await _factory.PlannedMeals.ListAsync(x => true, 1, int.MaxValue);
            await ExportAsync(meals, file);
        }

        /// <summary>
        /// Export a collection of planned meals to a CSV file
        /// </summary>
        /// <param name="meals"></param>
        /// <param name="file"></param>
#pragma warning disable CS1998
        public async Task ExportAsync(IEnumerable<PlannedMeal> meals, string file)
        {
            // Convert to exportable (flattened hierarchy) meals
            var exportable = meals.ToExportable();

            // Configure an exporter to export them
            var exporter = new CsvExporter<ExportablePlannedMeal>(ExportableEntityBase.DateTimeFormat);
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
        private void OnRecordExported(object _, ExportEventArgs<ExportablePlannedMeal> e)
        {
            RecordExport?.Invoke(this, e);
        }
    }
}

using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Interfaces;
using HealthTracker.DataExchange.Extensions;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Food;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DataExchange.Export
{
    [ExcludeFromCodeCoverage]
    public class MealFoodItemExporter : IMealFoodItemExporter
    {
        private readonly IHealthTrackerFactory _factory;

        public event EventHandler<ExportEventArgs<ExportableMealFoodItem>> RecordExport;

        public MealFoodItemExporter(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Export the meal/food item relationships to a CSV file
        /// </summary>
        /// <param name="file"></param>
        public async Task ExportAsync(string file)
        {
            var relationships = await _factory.MealFoodItems.ListAsync(x => true);
            await ExportAsync(relationships, file);
        }

        /// <summary>
        /// Export a collection of meal/food item relationships to a CSV file
        /// </summary>
        /// <param name="relationships"></param>
        /// <param name="file"></param>
        public async Task ExportAsync(IEnumerable<MealFoodItem> relationships, string file)
        {
            // Get the list of meals and food items for Id-to-name mapping
            var meals = await _factory.Meals.ListAsync(x => true, 1, int.MaxValue);
            var foodItems = await _factory.FoodItems.ListAsync(x => true, 1, int.MaxValue);

            // Convert to exportable (flattened hierarchy) mealfooditems
            var exportable = relationships.ToExportable(meals, foodItems);

            // Configure an exporter to export them
            var exporter = new CsvExporter<ExportableMealFoodItem>(null);
            exporter.RecordExport += OnRecordExported;

            // Export the records
            exporter.Export(exportable, file, ',');
        }

        /// <summary>
        /// Handler for meal/food item relationship export notifications
        /// </summary>
        /// <param name="_"></param>
        /// <param name="e"></param>
        private void OnRecordExported(object _, ExportEventArgs<ExportableMealFoodItem> e)
        {
            RecordExport?.Invoke(this, e);
        }
    }
}

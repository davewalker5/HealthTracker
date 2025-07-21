using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Food;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Interfaces;

namespace HealthTracker.DataExchange.Import
{
    public sealed class MealFoodItemImporter : CsvImporter<ExportableMealFoodItem>, IMealFoodItemImporter 
    {
        private List<Meal> _meals = [];
        private List<FoodItem> _foodItems = [];

        public MealFoodItemImporter(IHealthTrackerFactory factory, string format) : base(factory, format)
        {
        }

        /// <summary>
        /// Prepare for import
        /// </summary>
        /// <returns></returns>
        protected override async Task Prepare()
        {
            await base.Prepare();
            _meals = await _factory.Meals.ListAsync(x => true, 1, int.MaxValue);
            _foodItems = await _factory.FoodItems.ListAsync(x => true, 1, int.MaxValue);
        }

        /// <summary>
        /// Inflate a record to an object
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        protected override ExportableMealFoodItem Inflate(string record)
            => ExportableMealFoodItem.FromCsv(record);

        /// <summary>
        /// Validate an inflated record
        /// </summary>
        /// <param name="exportable"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        protected override void Validate(ExportableMealFoodItem exportable, int recordCount)
        {
            // Make sure the meal maps OK
            var meal = _meals.FirstOrDefault(x => x.Name.Equals(exportable.Meal, StringComparison.OrdinalIgnoreCase));
            ValidateField<Meal>(x => x != null, meal, "Meal", recordCount);

            // Make sure the food item maps OK
            var foodItem = _foodItems.FirstOrDefault(x => x.Name.Equals(exportable.FoodItem, StringComparison.OrdinalIgnoreCase));
            ValidateField<FoodItem>(x => x != null, foodItem, "FoodItem", recordCount);
        }

        /// <summary>
        /// Store an inflated record in the database
        /// </summary>
        /// <param name="exportable"></param>
        /// <returns></returns>
        protected override async Task AddAsync(ExportableMealFoodItem exportable)
        {
            // Get the IDs for the meal and food item
            var mealId = _meals.First(x => x.Name.Equals(exportable.Meal, StringComparison.OrdinalIgnoreCase)).Id;
            var foodItemId = _foodItems.First(x => x.Name.Equals(exportable.FoodItem, StringComparison.OrdinalIgnoreCase)).Id;

            // Save the relationship
            await _factory.MealFoodItems.AddAsync(mealId, foodItemId);
        }
    }
}
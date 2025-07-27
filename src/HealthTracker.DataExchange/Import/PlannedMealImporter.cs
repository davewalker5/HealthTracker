using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Food;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Interfaces;
using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.DataExchange.Import
{
    public sealed class PlannedMealImporter : CsvImporter<ExportablePlannedMeal>, IPlannedMealImporter 
    {
        private List<Meal> _meals = [];

        public PlannedMealImporter(IHealthTrackerFactory factory, string format) : base(factory, format)
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
        }

        /// <summary>
        /// Inflate a record to an object
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        protected override ExportablePlannedMeal Inflate(string record)
            => ExportablePlannedMeal.FromCsv(record);

        /// <summary>
        /// Validate an inflated record
        /// </summary>
        /// <param name="exportable"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        protected override void Validate(ExportablePlannedMeal exportable, int recordCount)
        {
            // Make sure the meal type is valid
            _ = Enum.TryParse(exportable.MealType, out MealType value);
            ValidateField<MealType>(x => (int)x > 0, value, "MealType", recordCount);

            // Make sure the meal maps OK
            var meal = _meals.FirstOrDefault(x => x.Name.Equals(exportable.Meal, StringComparison.OrdinalIgnoreCase));
            ValidateField<Meal>(x => x != null, meal, "Meal", recordCount);
        }

        /// <summary>
        /// Store an inflated record in the database
        /// </summary>
        /// <param name="exportable"></param>
        /// <returns></returns>
        protected override async Task AddAsync(ExportablePlannedMeal exportable)
        {
            // Parse the meal type out and find the related meal
            var mealType = (MealType)Enum.Parse(typeof(MealType), exportable.MealType);
            var meal = _meals.First(x => x.Name.Equals(exportable.Meal, StringComparison.OrdinalIgnoreCase));

            // Add the planned meal
            await _factory.PlannedMeals.AddAsync(mealType, exportable.Date, meal.Id);
        }
    }
}
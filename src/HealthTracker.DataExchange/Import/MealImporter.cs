using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Food;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Interfaces;

namespace HealthTracker.DataExchange.Import
{
    public sealed class MealImporter : CsvImporter<ExportableMeal>, IMealImporter 
    {
        private List<FoodSource> _foodSources = [];

        public MealImporter(IHealthTrackerFactory factory, string format) : base(factory, format)
        {
        }

        /// <summary>
        /// Prepare for import
        /// </summary>
        /// <returns></returns>
        protected override async Task Prepare()
        {
            await base.Prepare();
            _foodSources = await _factory.FoodSources.ListAsync(x => true, 1, int.MaxValue);
        }

        /// <summary>
        /// Inflate a record to an object
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        protected override ExportableMeal Inflate(string record)
            => ExportableMeal.FromCsv(record);

        /// <summary>
        /// Validate an inflated record
        /// </summary>
        /// <param name="meal"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        protected override void Validate(ExportableMeal meal, int recordCount)
        {
            // Make sure the food source maps OK
            var foodSource = _foodSources.FirstOrDefault(x => x.Name.Equals(meal.FoodSource, StringComparison.OrdinalIgnoreCase));
            ValidateField<FoodSource>(x => x != null, foodSource, "FoodSource", recordCount);

            // Validate the food item properties
            ValidateField<string>(x => !string.IsNullOrEmpty(x), meal.Name, "Name", recordCount);
            ValidateField<int>(x => x > 0, meal.Portions, "Portions", recordCount);
            ValidateField<decimal?>(x => !(x < 0), meal.Calories, "Calories", recordCount);
            ValidateField<decimal?>(x => !(x < 0), meal.Fat, "Fat", recordCount);
            ValidateField<decimal?>(x => !(x < 0), meal.SaturatedFat, "SaturatedFat", recordCount);
            ValidateField<decimal?>(x => !(x < 0), meal.Protein, "Protein", recordCount);
            ValidateField<decimal?>(x => !(x < 0), meal.Carbohydrates, "Carbohydrates", recordCount);
            ValidateField<decimal?>(x => !(x < 0), meal.Sugar, "Sugar", recordCount);
            ValidateField<decimal?>(x => !(x < 0), meal.Fibre, "Fibre", recordCount);
        }

        /// <summary>
        /// Store an inflated record in the database
        /// </summary>
        /// <param name="meal"></param>
        /// <returns></returns>
        protected override async Task AddAsync(ExportableMeal meal)
        {
            int? nutritionalValueId = null;

            // Get the related food source
            var source = _foodSources.First(x => x.Name.Equals(meal.FoodSource, StringComparison.OrdinalIgnoreCase));

            // If any of the nutritional vaues are set, create a nutritional value record from them
            if ((meal.Calories > 0) ||
                (meal.Fat > 0) ||
                (meal.SaturatedFat > 0) ||
                (meal.Protein > 0) ||
                (meal.Carbohydrates > 0) ||
                (meal.Sugar > 0) ||
                (meal.Fibre > 0))
            {
                var nutritionalValue = await _factory.NutritionalValues.AddAsync(meal.Calories, meal.Fat, meal.SaturatedFat, meal.Protein, meal.Carbohydrates, meal.Sugar, meal.Fibre);
                nutritionalValueId = nutritionalValue.Id;
            }

            // Add the food item
            await _factory.Meals.AddAsync(meal.Name, meal.Portions, source.Id, meal.Reference, nutritionalValueId);
        }
    }
}
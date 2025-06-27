using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Food;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Interfaces;

namespace HealthTracker.DataExchange.Import
{
    public sealed class FoodItemImporter : CsvImporter<ExportableFoodItem>, IFoodItemImporter 
    {
        private List<FoodCategory> _foodCategories = [];

        public FoodItemImporter(IHealthTrackerFactory factory, string format) : base(factory, format)
        {
        }

        /// <summary>
        /// Prepare for import
        /// </summary>
        /// <returns></returns>
        protected override async Task Prepare()
        {
            await base.Prepare();
            _foodCategories = await _factory.FoodCategories.ListAsync(x => true, 1, int.MaxValue);
        }

        /// <summary>
        /// Inflate a record to an object
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        protected override ExportableFoodItem Inflate(string record)
            => ExportableFoodItem.FromCsv(record);

        /// <summary>
        /// Validate an inflated record
        /// </summary>
        /// <param name="item"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        protected override void Validate(ExportableFoodItem item, int recordCount)
        {
            // Make sure the food category maps OK
            var foodCategory = _foodCategories.FirstOrDefault(x => x.Name.Equals(item.FoodCategory, StringComparison.OrdinalIgnoreCase));
            ValidateField<FoodCategory>(x => x != null, foodCategory, "FoodCategory", recordCount);

            // Validate the food item properties
            ValidateField<string>(x => !string.IsNullOrEmpty(x), item.Name, "Name", recordCount);
            ValidateField<decimal>(x => x > 0, item.Portion, "Portion", recordCount);
            ValidateField<decimal?>(x => !(x < 0), item.Calories, "Calories", recordCount);
            ValidateField<decimal?>(x => !(x < 0), item.Fat, "Fat", recordCount);
            ValidateField<decimal?>(x => !(x < 0), item.SaturatedFat, "SaturatedFat", recordCount);
            ValidateField<decimal?>(x => !(x < 0), item.Protein, "Protein", recordCount);
            ValidateField<decimal?>(x => !(x < 0), item.Carbohydrates, "Carbohydrates", recordCount);
            ValidateField<decimal?>(x => !(x < 0), item.Sugar, "Sugar", recordCount);
            ValidateField<decimal?>(x => !(x < 0), item.Fibre, "Fibre", recordCount);
        }

        /// <summary>
        /// Store an inflated record in the database
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override async Task AddAsync(ExportableFoodItem item)
        {
            int? nutritionalValueId = null;

            // Get the related food category
            var category = _foodCategories.First(x => x.Name.Equals(item.FoodCategory, StringComparison.OrdinalIgnoreCase));

            // If any of the nutritional vaues are set, create a nutritional value record from them
            if ((item.Calories > 0) ||
                (item.Fat > 0) ||
                (item.SaturatedFat > 0) ||
                (item.Protein > 0) ||
                (item.Carbohydrates > 0) ||
                (item.Sugar > 0) ||
                (item.Fibre > 0))
            {
                var nutritionalValue = await _factory.NutritionalValues.AddAsync(item.Calories, item.Fat, item.SaturatedFat, item.Protein, item.Carbohydrates, item.Sugar, item.Fibre);
                nutritionalValueId = nutritionalValue.Id;
            }

            // Add the food item
            await _factory.FoodItems.AddAsync(item.Name, item.Portion, category.Id, nutritionalValueId);
        }
    }
}
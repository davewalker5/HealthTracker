using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Food;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Interfaces;

namespace HealthTracker.DataExchange.Import
{
    public sealed class MealConsumptionMeasurementImporter : MeasurementImporter<ExportableMealConsumptionMeasurement>, IMealConsumptionMeasurementImporter
    {
        private List<Meal> _meals = [];

        public MealConsumptionMeasurementImporter(IHealthTrackerFactory factory, string format) : base (factory, format) {}

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
        protected override ExportableMealConsumptionMeasurement Inflate(string record)
            => ExportableMealConsumptionMeasurement.FromCsv(record);

        /// <summary>
        /// Validate an inflated record
        /// </summary>
        /// <param name="measurement"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        protected override void Validate(ExportableMealConsumptionMeasurement measurement, int recordCount)
        {
            ValidateCommonFields(measurement, recordCount);

            // Make sure the meal maps OK
            var meal = _meals.FirstOrDefault(x => x.Name.Equals(measurement.Meal, StringComparison.OrdinalIgnoreCase));
            ValidateField<Meal>(x => x != null, meal, "Meal", recordCount);

            ValidateField<decimal>(x => x > 0, measurement.Quantity, "Quantity", recordCount);
            ValidateField<decimal?>(x => !(x < 0), measurement.Calories, "Calories", recordCount);
            ValidateField<decimal?>(x => !(x < 0), measurement.Fat, "Fat", recordCount);
            ValidateField<decimal?>(x => !(x < 0), measurement.SaturatedFat, "SaturatedFat", recordCount);
            ValidateField<decimal?>(x => !(x < 0), measurement.Protein, "Protein", recordCount);
            ValidateField<decimal?>(x => !(x < 0), measurement.Carbohydrates, "Carbohydrates", recordCount);
            ValidateField<decimal?>(x => !(x < 0), measurement.Sugar, "Sugar", recordCount);
            ValidateField<decimal?>(x => !(x < 0), measurement.Fibre, "Fibre", recordCount);
        }

        /// <summary>
        /// Store an inflated record in the database
        /// </summary>
        /// <param name="measurement"></param>
        /// <returns></returns>
        protected override async Task AddAsync(ExportableMealConsumptionMeasurement measurement)
        {
            int? nutritionalValueId = null;

            // Get the related meal
            var meal = _meals.First(x => x.Name.Equals(measurement.Meal, StringComparison.OrdinalIgnoreCase));

            // If any of the nutritional vaues are set, create a nutritional value record from them
            if ((measurement.Calories > 0) ||
                (measurement.Fat > 0) ||
                (measurement.SaturatedFat > 0) ||
                (measurement.Protein > 0) ||
                (measurement.Carbohydrates > 0) ||
                (measurement.Sugar > 0) ||
                (measurement.Fibre > 0))
            {
                var nutritionalValue = await _factory.NutritionalValues.AddAsync(measurement.Calories, measurement.Fat, measurement.SaturatedFat, measurement.Protein, measurement.Carbohydrates, measurement.Sugar, measurement.Fibre);
                nutritionalValueId = nutritionalValue.Id;
            }

            await _factory.MealConsumptionMeasurements.AddAsync(
                measurement.PersonId,
                meal.Id,
                nutritionalValueId,
                measurement.Date,
                measurement.Quantity);
        }
    }
}
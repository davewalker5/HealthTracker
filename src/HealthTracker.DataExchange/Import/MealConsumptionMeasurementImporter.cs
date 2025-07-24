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

            // On export, the nutritional values associated with the consumption record are also exported. On import, though,
            // the nutritional values are recalculated and added automatically based on the current state of the specified
            // meal, so only the quantity is validated
            ValidateField<decimal>(x => x > 0, measurement.Quantity, "Quantity", recordCount);
        }

        /// <summary>
        /// Store an inflated record in the database
        /// </summary>
        /// <param name="measurement"></param>
        /// <returns></returns>
        protected override async Task AddAsync(ExportableMealConsumptionMeasurement measurement)
        {
            // Get the related meal
            var meal = _meals.First(x => x.Name.Equals(measurement.Meal, StringComparison.OrdinalIgnoreCase));

            // On export, the nutritional values associated with the consumption record are also exported. On import, though,
            // the nutritional values are recalculated and added automatically based on the current state of the specified
            // meal, so there's no need to manually create a nutritional value record. Just add the consumption record
            await _factory.MealConsumptionMeasurements.AddAsync(
                measurement.PersonId,
                meal.Id,
                measurement.Date,
                measurement.Quantity);
        }
    }
}
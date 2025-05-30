using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Interfaces;
using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.DataExchange.Import
{
    public sealed class BeverageConsumptionMeasurementImporter : MeasurementImporter<ExportableBeverageConsumptionMeasurement>, IBeverageConsumptionMeasurementImporter
    {
        private List<Beverage> _beverages = [];

        public BeverageConsumptionMeasurementImporter(IHealthTrackerFactory factory, string format) : base (factory, format) {}

        /// <summary>
        /// Prepare for import
        /// </summary>
        /// <returns></returns>
        protected override async Task Prepare()
        {
            await base.Prepare();
            _beverages = await _factory.Beverages.ListAsync(x => true, 1, int.MaxValue);
        }

        /// <summary>
        /// Inflate a record to an object
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        protected override ExportableBeverageConsumptionMeasurement Inflate(string record)
            => ExportableBeverageConsumptionMeasurement.FromCsv(record);

        /// <summary>
        /// Validate an inflated record
        /// </summary>
        /// <param name="measurement"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        protected override void ValidateAsync(ExportableBeverageConsumptionMeasurement measurement, int recordCount)
        {
            ValidateCommonFields(measurement, recordCount);

            // Make sure the beverage maps OK
            var beverage = _beverages.FirstOrDefault(x => x.Name.Equals(measurement.Beverage, StringComparison.OrdinalIgnoreCase));
            ValidateField<Beverage>(x => x != null, beverage, "Beverage", recordCount);

            ValidateField<int>(x => x != (int)BeverageMeasure.None, measurement.Measure, "Measure", recordCount);
            ValidateField<int>(x => x > 0, measurement.Quantity, "Quantity", recordCount);
            ValidateField<decimal>(x => x >= 0 && x <= 100, measurement.ABV, "ABV", recordCount);
        }

        /// <summary>
        /// Store an inflated record in the database
        /// </summary>
        /// <param name="measurement"></param>
        /// <returns></returns>
        protected override async Task AddAsync(ExportableBeverageConsumptionMeasurement measurement)
        {
            var beverage = _beverages.First(x => x.Name.Equals(measurement.Beverage, StringComparison.OrdinalIgnoreCase));
            await _factory.BeverageConsumptionMeasurements.AddAsync(
                measurement.PersonId,
                beverage.Id,
                measurement.Date,
                (BeverageMeasure)measurement.Measure,
                measurement.Quantity,
                measurement.ABV);
        }
    }
}
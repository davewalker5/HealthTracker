using HealthTracker.Entities.Interfaces;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Interfaces;

namespace HealthTracker.DataExchange.Import
{
    public sealed class WeightMeasurementImporter : MeasurementImporter<ExportableWeightMeasurement>, IWeightMeasurementImporter
    {
        public WeightMeasurementImporter(IHealthTrackerFactory factory, string format) : base (factory, format, false, true) {}

        /// <summary>
        /// Inflate a record to an object
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        protected override ExportableWeightMeasurement Inflate(string record)
            => ExportableWeightMeasurement.FromCsv(record);

        /// <summary>
        /// Prepare for import
        /// </summary>
        /// <returns></returns>
        protected override async Task Prepare()
            => await base.Prepare();

        /// <summary>
        /// Validate an inflated record
        /// </summary>
        /// <param name="measurement"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        protected override void Validate(ExportableWeightMeasurement measurement, int recordCount)
        {
            ValidateCommonFields(measurement, recordCount);
            ValidateField<decimal>(x => x > 0, measurement.Weight, "Weight", recordCount);
        }

        /// <summary>
        /// Store an inflated record in the database
        /// </summary>
        /// <param name="measurement"></param>
        /// <returns></returns>
        protected override async Task AddAsync(ExportableWeightMeasurement measurement)
            => await _factory.WeightMeasurements.AddAsync(measurement.PersonId, measurement.Date, measurement.Weight);
    }
}
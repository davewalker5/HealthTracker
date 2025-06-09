using HealthTracker.Entities.Interfaces;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Interfaces;

namespace HealthTracker.DataExchange.Import
{
    public sealed class BloodOxygenSaturationMeasurementImporter : MeasurementImporter<ExportableBloodOxygenSaturationMeasurement>, IBloodOxygenSaturationMeasurementImporter
    {
        public BloodOxygenSaturationMeasurementImporter(IHealthTrackerFactory factory, string format) : base (factory, format) {}

        /// <summary>
        /// Inflate a record to an object
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        protected override ExportableBloodOxygenSaturationMeasurement Inflate(string record)
            => ExportableBloodOxygenSaturationMeasurement.FromCsv(record);

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
        protected override void Validate(ExportableBloodOxygenSaturationMeasurement measurement, int recordCount)
        {
            ValidateCommonFields(measurement, recordCount);
            ValidateField<decimal>(x => x >= 0M, measurement.Percentage, "Percentage", recordCount);
        }

        /// <summary>
        /// Store an inflated record in the database
        /// </summary>
        /// <param name="measurement"></param>
        /// <returns></returns>
        protected override async Task AddAsync(ExportableBloodOxygenSaturationMeasurement measurement)
            => await _factory.BloodOxygenSaturationMeasurements.AddAsync(
                measurement.PersonId,
                measurement.Date,
                measurement.Percentage);
    }
}
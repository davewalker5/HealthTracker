using HealthTracker.Entities.Interfaces;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Interfaces;

namespace HealthTracker.DataExchange.Import
{
    public sealed class CholesterolMeasurementImporter : MeasurementImporter<ExportableCholesterolMeasurement>, ICholesterolMeasurementImporter
    {
        public CholesterolMeasurementImporter(IHealthTrackerFactory factory, string format) : base (factory, format) {}

        /// <summary>
        /// Inflate a record to an object
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        protected override ExportableCholesterolMeasurement Inflate(string record)
            => ExportableCholesterolMeasurement.FromCsv(record);

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
        protected override void Validate(ExportableCholesterolMeasurement measurement, int recordCount)
        {
            ValidateCommonFields(measurement, recordCount);
            ValidateField<decimal>(x => x > 0, measurement.Total, "Total", recordCount);
            ValidateField<decimal>(x => x > 0, measurement.HDL, "HDL", recordCount);
            ValidateField<decimal>(x => x > 0, measurement.LDL, "LDL", recordCount);
            ValidateField<decimal>(x => x > 0, measurement.Triglycerides, "Triglycerides", recordCount);
        }

        /// <summary>
        /// Store an inflated record in the database
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        protected override async Task AddAsync(ExportableCholesterolMeasurement measurement)
            => await _factory.CholesterolMeasurements.AddAsync(
                measurement.PersonId,
                measurement.Date,
                measurement.Total,
                measurement.HDL,
                measurement.LDL,
                measurement.Triglycerides);
    }
}
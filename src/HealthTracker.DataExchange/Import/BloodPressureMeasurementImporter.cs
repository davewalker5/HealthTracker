using HealthTracker.Entities.Interfaces;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Interfaces;

namespace HealthTracker.DataExchange.Import
{
    public sealed class BloodPressureMeasurementImporter : MeasurementImporter<ExportableBloodPressureMeasurement>, IBloodPressureMeasurementImporter
    {
        public BloodPressureMeasurementImporter(IHealthTrackerFactory factory, string format) : base (factory, format, false, true) {}

        /// <summary>
        /// Inflate a record to an object
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        protected override ExportableBloodPressureMeasurement Inflate(string record)
            => ExportableBloodPressureMeasurement.FromCsv(record);

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
        protected override void Validate(ExportableBloodPressureMeasurement measurement, int recordCount)
        {
            ValidateCommonFields(measurement, recordCount);
            ValidateField<int>(x => x > 0, measurement.Systolic, "Systolic", recordCount);
            ValidateField<int>(x => x > 0, measurement.Diastolic, "Diastolic", recordCount);
        }

        /// <summary>
        /// Store an inflated record in the database
        /// </summary>
        /// <param name="measurement"></param>
        /// <returns></returns>
        protected override async Task AddAsync(ExportableBloodPressureMeasurement measurement)
            => await _factory.BloodPressureMeasurements.AddAsync(
                measurement.PersonId,
                measurement.Date,
                measurement.Systolic,
                measurement.Diastolic);
    }
}
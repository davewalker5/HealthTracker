using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Interfaces;
using HealthTracker.Logic.Extensions;

namespace HealthTracker.DataExchange.Import
{
    public sealed class ExerciseMeasurementImporter : MeasurementImporter<ExportableExerciseMeasurement>, IExerciseMeasurementImporter
    {
        private List<ActivityType> _activityTypes = [];

        public ExerciseMeasurementImporter(IHealthTrackerFactory factory, string format) : base (factory, format) {}

        /// <summary>
        /// Prepare for import
        /// </summary>
        /// <returns></returns>
        protected override async Task Prepare()
        {
            await base.Prepare();
            _activityTypes = await _factory.ActivityTypes.ListAsync(x => true, 1, int.MaxValue);
        }

        /// <summary>
        /// Inflate a record to an object
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        protected override ExportableExerciseMeasurement Inflate(string record)
            => ExportableExerciseMeasurement.FromCsv(record);

        /// <summary>
        /// Validate an inflated record
        /// </summary>
        /// <param name="measurement"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        protected override void Validate(ExportableExerciseMeasurement measurement, int recordCount)
        {
            ValidateCommonFields(measurement, recordCount);

            // Make sure the activity type maps OK
            var activityType = _activityTypes.FirstOrDefault(x => x.Description.Equals(measurement.ActivityType, StringComparison.OrdinalIgnoreCase));
            ValidateField<ActivityType>(x => x != null, activityType, "ActivityType", recordCount);

            // The extension method to convert HH:MM:SS to a duration will throw an exception if the field's in the wrong format
            var duration = measurement.Duration.ToDuration();
            ValidateField<int>(x => x > 0, duration, "Duration", recordCount);

            // Distance MAY be null or 0 (for no value) or > 0 for a valid duration
            ValidateField<decimal?>(x => !(x < 0), measurement.Distance, "Distance", recordCount);

            ValidateField<int>(x => x >= 0, measurement.Calories, "Calories", recordCount);
            ValidateField<int>(x => x >= 0, measurement.MinimumHeartRate, "MinimumHeartRate", recordCount);
            ValidateField<int>(x => x >= 0, measurement.MaximumHeartRate, "MaximumHeartRate", recordCount);
            ValidateField<int>(x => x >= measurement.MinimumHeartRate, measurement.MaximumHeartRate, "MaximumHeartRate", recordCount);
        }

        /// <summary>
        /// Store an inflated record in the database
        /// </summary>
        /// <param name="measurement"></param>
        /// <returns></returns>
        protected override async Task AddAsync(ExportableExerciseMeasurement measurement)
        {
            var activityType = _activityTypes.First(x => x.Description.Equals(measurement.ActivityType, StringComparison.OrdinalIgnoreCase));
            await _factory.ExerciseMeasurements.AddAsync(
                measurement.PersonId,
                activityType.Id,
                measurement.Date,
                measurement.Duration.ToDuration(),
                measurement.Distance,
                measurement.Calories,
                measurement.MinimumHeartRate,
                measurement.MaximumHeartRate);
        }
    }
}
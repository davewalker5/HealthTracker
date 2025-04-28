using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;

namespace HealthTracker.Logic.Calculations
{
    public class BloodPressureAssessor : IBloodPressureAssessor
    {
        private readonly IHealthTrackerFactory _factory;

        internal BloodPressureAssessor(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Assess each blood pressure measurement in a collection of measurements
        /// </summary>
        /// <param name="measurements"></param>
        /// <returns></returns>
        public async Task Assess(IEnumerable<BloodPressureMeasurement> measurements)
        {
            // Load the assessment bands. They are returned in "assessment" order and are expected
            // to be defined in most-to-least-serious order
            var bands = await _factory.BloodPressureBands.ListAsync(x => true);

            foreach (var measurement in measurements)
            {
                BloodPressureBand matched = null;

                // Iterate over the bands to find the one that matches
                for (int i = 0; (matched == null) && (i < bands.Count); i++)
                {
                    if (MatchesBand(measurement, bands[i]))
                    {
                        matched = bands[i];
                    }
                }

                // Apply the assessment to the measurement
                measurement.Assessment = matched?.Name;
            }
        }

        /// <summary>
        /// Return true if a measurement matches an assessment band
        /// </summary>
        /// <param name="measurement"></param>
        /// <param name="band"></param>
        /// <returns></returns>
        private static bool MatchesBand(BloodPressureMeasurement measurement, BloodPressureBand band)
            =>  (band.MatchAll && MatchesSystolicRange(measurement, band) && MatchesDiastolicRange(measurement, band)) ||
                (!band.MatchAll && (MatchesSystolicRange(measurement, band) || MatchesDiastolicRange(measurement, band)));

        /// <summary>
        /// Return true if a measurement matches the range of systolic blood pressure values given in an assessment band
        /// </summary>
        /// <param name="measurement"></param>
        /// <param name="band"></param>
        /// <returns></returns>
        private static bool MatchesSystolicRange(BloodPressureMeasurement measurement, BloodPressureBand band)
            => (measurement.Systolic >= band.MinimumSystolic) && (measurement.Systolic <= band.MaximumSystolic);

        /// <summary>
        /// Return true if a measurement matches the range of systolic blood pressure values given in an assessment band
        /// </summary>
        /// <param name="measurement"></param>
        /// <param name="band"></param>
        /// <returns></returns>
        private static bool MatchesDiastolicRange(BloodPressureMeasurement measurement, BloodPressureBand band)
            => (measurement.Diastolic >= band.MinimumDiastolic) && (measurement.Diastolic <= band.MaximumDiastolic);
    }
}

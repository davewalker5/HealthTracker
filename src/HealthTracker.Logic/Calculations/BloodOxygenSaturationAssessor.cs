using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;

namespace HealthTracker.Logic.Calculations
{
    public class BloodOxygenSaturationAssessor : IBloodOxygenSaturationAssessor
    {
        private readonly IHealthTrackerFactory _factory;

        internal BloodOxygenSaturationAssessor(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Assess each blood oxygen saturation measurement in a collection of measurements
        /// </summary>
        /// <param name="measurements"></param>
        /// <returns></returns>
        public async Task Assess(IEnumerable<BloodOxygenSaturationMeasurement> measurements)
        {
            // Load the %SPO2 bands
            var bands = await _factory.BloodOxygenSaturationBands.ListAsync(x => true);

            // Calculate mappings between person IDs and ages in years
            var mapping = await CalculateIdToAgeMap();

            foreach (var measurement in measurements)
            {
                BloodOxygenSaturationBand matched = null;

                // Get the age of the person associated with the measurement
                var age = mapping[measurement.PersonId];

                // Iterate over the bands to find the one that matches
                for (int i = 0; (matched == null) && (i < bands.Count); i++)
                {
                    if (MatchesBand(measurement, bands[i], age))
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
        private static bool MatchesBand(BloodOxygenSaturationMeasurement measurement, BloodOxygenSaturationBand band, int age)
            =>  (age >= band.MinimumAge) && (age <= band.MaximumAge) && (measurement.Percentage >= band.MinimumSPO2) && (measurement.Percentage <= band.MaximumSPO2);

        /// <summary>
        /// Map person ages to person IDs
        /// </summary>
        /// <returns></returns>
        private async Task<Dictionary<int, int>> CalculateIdToAgeMap()
        {
            Dictionary<int, int> mapping = [];

            // Retrieve a list of all people in the database
            var people = await _factory.People.ListAsync(x => true, 1, int.MaxValue);

            // Iterate over them and, for each, add a mapping between their ID and age in years, with the ID
            // as the key
            foreach (var person in people)
            {
                mapping.Add(person.Id, person.AgeInYears());
            }

            return mapping;
        }
    }
}

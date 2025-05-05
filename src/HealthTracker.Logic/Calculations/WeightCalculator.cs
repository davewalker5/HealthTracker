using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.Logic.Calculations
{
    public class WeightCalculator : IWeightCalculator
    {
        private readonly IHealthTrackerFactory _factory;

        internal WeightCalculator(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Return an average weight for all readings between two dates
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<WeightMeasurement> AverageAsync(int personId, DateTime from, DateTime to)
        {
            WeightMeasurement average = null;
            var measurements = await _factory.WeightMeasurements.ListAsync(x => (x.PersonId == personId) && (x.Date >= from) && (x.Date <= to));

            if (measurements.Any())
            {
                average = new WeightMeasurement
                {
                    PersonId = personId,
                    Date = to,
                    Weight = Math.Round(measurements.Select(x => x.Weight).Average(), 2, MidpointRounding.AwayFromZero)
                };
            }

            return average;
        }

        /// <summary>
        /// Calculate the weight-related properties for a collection of weight measurements
        /// </summary>
        /// <param name="measurements"></param>
        /// <returns></returns>
        public async Task CalculateRelatedProperties(IEnumerable<WeightMeasurement> measurements)
        {
            // Load the list of people to provide height measurements
            var people = await _factory.People.ListAsync(x => true, 1, int.MaxValue);

            // Load the BMI bandings. . They are returned in "assessment" order and are expected
            // to be defined in lowest-to-highest BMI order
            var bands = await _factory.BMIBands.ListAsync(x => true);

            foreach (var measurement in measurements)
            {
                CalculateBMI(measurement, people, bands);
                CalculateBMR(measurement, people);
            }
        }

        /// <summary>
        /// Calculate th BMI and its banding for a single weight measurement
        /// </summary>
        /// <param name="measurement"></param>
        /// <param name="people"></param>
        /// <param name="bands"></param>
        private static void CalculateBMI(WeightMeasurement measurement, IList<Person> people, IList<BMIBand> bands)
        {
            // Find the person this measurement relates to
            var person = people.First(x => x.Id == measurement.PersonId);

            // Weight is assumed to be in kg and height in metres
            measurement.BMI = measurement.Weight / (person.Height * person.Height);
            Console.WriteLine($"Person height = {person.Height}, weight = {measurement.Weight}, bmi = {measurement.BMI}");

            // Determine which band the BMI belongs to
            BMIBand matched = null;
            for (int i = 0; (matched == null) && (i < bands.Count); i++)
            {
                if (measurement.BMI <= bands[i].MaximumBMI)
                {
                    matched = bands[i];
                }
            }

            // Apply the BMI assessment
            measurement.BMIAssessment = matched?.Name ?? "";
        }

        /// <summary>
        /// Calculate the BMR for a single weight measurement using the Harris-Benedict Equation
        /// </summary>
        /// <param name="measurement"></param>
        /// <param name="people"></param>
        private static void CalculateBMR(WeightMeasurement measurement, IList<Person> people)
        {
            // Find the person this measurement relates to
            var person = people.First(x => x.Id == measurement.PersonId);

            // The calculation is gender-specific
            measurement.BMR = person.Gender switch
            {
                Gender.Male => (13.7516M * measurement.Weight) + (5.0033M * person.Height * 100M) - (6.755M * person.AgeInYears()) + 66.473M,
                Gender.Female => (9.5634M * measurement.Weight) + (1.8496M * person.Height * 100M) - (4.6756M * person.AgeInYears()) + 655.0955M,
                _ => 0M
            };
        }
    }
}
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;

namespace HealthTracker.Logic.Calculations
{
    public class BloodOxygenSaturationCalculator : IBloodOxygenSaturationCalculator
    {
        private readonly IHealthTrackerFactory _factory;

        internal BloodOxygenSaturationCalculator(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Return an average % SPO2 reading between two dates
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<BloodOxygenSaturationMeasurement> AverageAsync(int personId, DateTime from, DateTime to)
        {
            BloodOxygenSaturationMeasurement average = null;
            var measurements = await _factory.BloodOxygenSaturationMeasurements.ListAsync(x => (x.PersonId == personId) && (x.Date >= from) && (x.Date <= to));

            if (measurements.Any())
            {
                average = new BloodOxygenSaturationMeasurement
                {
                    PersonId = personId,
                    Date = to,
                    Percentage = Math.Round(measurements.Select(x => x.Percentage).Average(), 2, MidpointRounding.AwayFromZero)
                };
            }

            return average;
        }

        /// <summary>
        /// Calculate a daily average % SPO2 reading for each date in a date range
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<List<BloodOxygenSaturationMeasurement>> DailyAverageAsync(int personId, DateTime from, DateTime to)
        {
            List<BloodOxygenSaturationMeasurement> averages = null;

            // Retrieve the measurements lying in the requested date range
            var measurements = await _factory.BloodOxygenSaturationMeasurements.ListAsync(x => (x.PersonId == personId) && (x.Date >= from) && (x.Date <= to));
            if (measurements.Any())
            {
                averages = [];

                // Compile a list of distinct dates with the time reset to 00:00:00 and iterate over those dates
                var dates = measurements.Select(x => new DateTime(x.Date.Year, x.Date.Month, x.Date.Day, 0, 0, 0)).Distinct();
                foreach (var date in dates)
                {
                    // Get a list of measurements for this date
                    var endDate = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
                    var dailyReadings = measurements.Where(x => (x.Date >= date) && (x.Date <= endDate));

                    // Calculate the daily average
                    var dailyAverage = new BloodOxygenSaturationMeasurement
                    {
                        PersonId = personId,
                        Date = date,
                        Percentage = Math.Round(dailyReadings.Select(x => x.Percentage).Average(), 2, MidpointRounding.AwayFromZero)
                    };

                    // Add the daily average to the collection
                    averages.Add(dailyAverage);
                }
            }

            return averages;
        }
    }
}

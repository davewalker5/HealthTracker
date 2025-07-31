using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Food;

namespace HealthTracker.Logic.Calculations
{
    public class MealConsumptionCalculator : IMealConsumptionCalculator
    {
        private readonly IHealthTrackerFactory _factory;

        internal MealConsumptionCalculator(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Calculate a daily total meal consumption for each date in a date range
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<List<MealConsumptionDailySummary>> DailyTotalConsumptionAsync(int personId, DateTime from, DateTime to)
        {
            List<MealConsumptionDailySummary> totals = [];

            // Retrieve matching measurements
            var measurements = await _factory.MealConsumptionMeasurements.ListAsync(x => 
                    (x.PersonId == personId) &&
                    (x.Date >= from) &&
                    (x.Date <= to),
                    1, int.MaxValue);
            if (measurements.Any())
            {
                // Get a list of people so the person name can be populated
                var people = await _factory.People.ListAsync(x => true, 1, int.MaxValue);

                // Compile a list of distinct dates with the time reset to 00:00:00 and iterate over those dates
                var dates = measurements.Select(x => new DateTime(x.Date.Year, x.Date.Month, x.Date.Day, 0, 0, 0)).Distinct();
                foreach (var date in dates)
                {
                    // Get a list of measurements for this date
                    var endDate = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
                    var dailyReadings = measurements.Where(x => (x.Date >= date) && (x.Date <= endDate));

                    // Calculate the daily total
                    var dailyTotal = new MealConsumptionDailySummary
                    {
                        PersonId = personId,
                        PersonName = people.First(x => x.Id == personId).Name,
                        Date = date,
                        Calories = dailyReadings.Where(x => x.NutritionalValue.Calories != null).Sum(x => x.NutritionalValue.Calories),
                        Fat = dailyReadings.Where(x => x.NutritionalValue.Fat != null).Sum(x => x.NutritionalValue.Fat),
                        SaturatedFat = dailyReadings.Where(x => x.NutritionalValue.SaturatedFat != null).Sum(x => x.NutritionalValue.SaturatedFat),
                        Protein = dailyReadings.Where(x => x.NutritionalValue.Protein != null).Sum(x => x.NutritionalValue.Protein),
                        Carbohydrates = dailyReadings.Where(x => x.NutritionalValue.Carbohydrates != null).Sum(x => x.NutritionalValue.Carbohydrates),
                        Sugar = dailyReadings.Where(x => x.NutritionalValue.Sugar != null).Sum(x => x.NutritionalValue.Sugar),
                        Fibre = dailyReadings.Where(x => x.NutritionalValue.Fibre != null).Sum(x => x.NutritionalValue.Fibre)
                    };

                    // Add the daily total to the collection
                    totals.Add(dailyTotal);
                }
            }

            return totals;
        }
    }
}

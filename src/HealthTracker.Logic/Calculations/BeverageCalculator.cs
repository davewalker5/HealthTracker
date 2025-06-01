using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.Entities.Logging;

namespace HealthTracker.Logic.Calculations
{
    public class BeverageConsumptionCalculator : IBeverageConsumptionCalculator
    {
        private readonly IHealthTrackerFactory _factory;

        internal BeverageConsumptionCalculator(IHealthTrackerFactory factory)
            => _factory = factory;

        /// <summary>
        /// Calculate a daily total hydrating beverage consumption for each date in a date range
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public async Task<List<BeverageConsumptionSummary>> DailyTotalHydratingAsync(int personId, DateTime from, DateTime to)
            => await DailyConsumptionAsync(personId, from, to, true, false);

        /// <summary>
        /// Calculate a daily total alcohol consumption for each date in a date range
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public async Task<List<BeverageConsumptionSummary>> DailyTotalAlcoholAsync(int personId, DateTime from, DateTime to)
            => await DailyConsumptionAsync(personId, from, to, false, true);

        /// <summary>
        /// Calculate a total hydrating beverage consumption for a date range
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public async Task<BeverageConsumptionSummary> TotalHydratingAsync(int personId, DateTime from, DateTime to)
            => await TotalConsumptionAsync(personId, from, to, true, false);

        /// <summary>
        /// Calculate a total alcohol consumption for a date range
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public async Task<BeverageConsumptionSummary> TotalAlcoholAsync(int personId, DateTime from, DateTime to)
            => await TotalConsumptionAsync(personId, from, to, false, true);

        /// <summary>
        /// Calculate a daily average beverage consumption for each date in a date range
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="hydrating"></param>
        /// <param name="alcohol"></param>
        /// <returns></returns>
        private async Task<List<BeverageConsumptionSummary>> DailyConsumptionAsync(int personId, DateTime from, DateTime to, bool hydrating, bool alcohol)
        {
            List<BeverageConsumptionSummary> totals = [];

            // Retrieve matching measurements
            var measurements = await RetrieveMeasurements(personId, from, to, hydrating, alcohol);
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

                    // Get the person and beverage details
                    (int? beverageId, string beverageName) = GetBeverage(dailyReadings);

                    // Calculate the daily total
                    var dailyTotal = new BeverageConsumptionSummary
                    {
                        PersonId = personId,
                        PersonName = people.First(x => x.Id == personId).Name,
                        From = from,
                        To = to,
                        BeverageId = beverageId,
                        BeverageName = beverageName,
                        TotalVolume = dailyReadings.Select(x => x.Volume).Sum(),
                        TotalUnits = dailyReadings.Select(x => x.Units).Sum()
                    };

                    // Add the daily total to the collection
                    totals.Add(dailyTotal);
                }
            }

            return totals;
        }

        /// <summary>
        /// Calculate a total beverage consumption for a date range
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="hydrating"></param>
        /// <param name="alcohol"></param>
        /// <returns></returns>
        private async Task<BeverageConsumptionSummary> TotalConsumptionAsync(int personId, DateTime from, DateTime to, bool hydrating, bool alcohol)
        {
            BeverageConsumptionSummary total = null;

            // Retrieve matching measurements
            var measurements = await RetrieveMeasurements(personId, from, to, hydrating, alcohol);
            if (measurements.Any())
            {
                // Get a list of people so the person name can be populated
                var people = await _factory.People.ListAsync(x => true, 1, int.MaxValue);

                // Get the beverage details
                (int? beverageId, string beverageName) = GetBeverage(measurements);

                // Construct the total consumption object
                total = new BeverageConsumptionSummary
                {
                    PersonId = personId,
                    PersonName = people.First(x => x.Id == personId).Name,
                    From = from,
                    To = to,
                    BeverageId = beverageId,
                    BeverageName = beverageName,
                    TotalVolume = measurements.Select(x => x.Volume).Sum(),
                    TotalUnits = measurements.Select(x => x.Units).Sum()
                };
            }

            return total;
        }

        /// <summary>
        /// Determine the beverage for a collection of measurements
        /// </summary>
        /// <param name="measurements"></param>
        /// <returns></returns>
        private static (int?, string) GetBeverage(IEnumerable<BeverageConsumptionMeasurement> measurements)
        {
            // If there's only one beverage in the collection, that's the one, otherwise return a null/empty beverage
            var beverageIds = measurements.Select(x => x.BeverageId).Distinct();
            int? beverageId = beverageIds.Count() == 1 ? beverageIds.First() : null;
            var beverage = beverageId != null ? measurements.FirstOrDefault(x => x.BeverageId == beverageId).Beverage : "";
            return (beverageId, beverage);
        }

        /// <summary>
        /// Retrieve a set of beverage consumption measurements for a person and date range
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="hydrating"></param>
        /// <param name="alcohol"></param>
        /// <returns></returns>
        private async Task<List<BeverageConsumptionMeasurement>> RetrieveMeasurements(int personId, DateTime from, DateTime to, bool hydrating, bool alcohol)
        {
            List<BeverageConsumptionMeasurement> measurements = [];

            _factory.Logger.LogMessage(Severity.Debug, $"Retrieving beverage consumption measurements for person ID {personId} between {from} and {to}");

            // Retrieve a list of beverages and a list of those matching the input critera
            var beverages = await _factory.Beverages.ListAsync(x => true, 1, int.MaxValue);
            var beverageIds = beverages.Where(x => (x.IsHydrating == hydrating) && (x.IsAlcohol == alcohol)).Select(x => x.Id);

            _factory.Logger.LogMessage(Severity.Debug, $"{beverageIds.Count()} beverages found matching Hydrating = {hydrating}, Alcoholic = {alcohol}");

            if (beverageIds.Any())
            {
                // Retrieve the measurements lying in the requested date range for the matching beverages
                measurements = await _factory.BeverageConsumptionMeasurements.ListAsync(x => 
                    (x.PersonId == personId) &&
                    (x.Date >= from) &&
                    (x.Date <= to) &&
                    beverageIds.Contains(x.BeverageId),
                    1, int.MaxValue);

                if (measurements.Any())
                {
                    _factory.Logger.LogMessage(Severity.Debug, $"{measurements.Count()} matching measurements found");

                    // Calculate the number of units of alcohol for each
                    _factory.AlcoholUnitsCalculator.CalculateUnits(measurements);

                    // Populate the beverage names
                    foreach (var measurement in measurements)
                    {
                        measurement.Beverage = beverages.First(x => x.Id == measurement.BeverageId).Name;
                    }
                }
                else
                {
                    _factory.Logger.LogMessage(Severity.Debug, $"No matching measurements found");
                }
            }

            return measurements;
        }
    }
}

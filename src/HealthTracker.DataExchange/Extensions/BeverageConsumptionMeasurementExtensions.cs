using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Measurements;
using HealthTracker.DataExchange.Entities;

namespace HealthTracker.DataExchange.Extensions
{
    public static class BeverageConsumptionMeasurementExtensions
    {
        /// <summary>
        /// Return an exportable beverage consumption measurement from an alcohol consumption measuement
        /// </summary>
        /// <param name="measurement"></param>
        /// <param name="people"></param>
        /// <param name="beverages"></param>
        /// <returns></returns>
        public static ExportableBeverageConsumptionMeasurement ToExportable(this BeverageConsumptionMeasurement measurement, IEnumerable<Person> people, IEnumerable<Beverage> beverages)
        {
            var person = people.First(x => x.Id == measurement.PersonId);
            var beverage = beverages.First(x => x.Id == measurement.BeverageId);
            return new ExportableBeverageConsumptionMeasurement
            {
                PersonId = measurement.PersonId,
                Name = $"{person.FirstNames} {person.Surname}",
                Date = measurement.Date,
                BeverageId = measurement.BeverageId,
                Beverage = beverage.Name,
                Quantity = measurement.Quantity,
                Volume = measurement.Volume,
                ABV = measurement.ABV,
                Units = measurement.Units
            };
        }

        /// <summary>
        /// Return a collection of exportable beverage consumption measurements from a collection of alcohol consumption measurements
        /// </summary>
        /// <param name="measurements"></param>
        /// <param name="people"></param>
        /// <param name="beverages"></param>
        /// <returns></returns>
        public static IEnumerable<ExportableBeverageConsumptionMeasurement> ToExportable(
            this IEnumerable<BeverageConsumptionMeasurement> measurements,
            IEnumerable<Person> people,
            IEnumerable<Beverage> beverages)
        {
            var exportable = new List<ExportableBeverageConsumptionMeasurement>();

            foreach (var measurement in measurements)
            {
                exportable.Add(measurement.ToExportable(people, beverages));
            }

            return exportable;
        }

        /// <summary>
        /// Return a beverage consumption measurement from an exportable beverage consumption measurement
        /// </summary>
        /// <param name="exportable"></param>
        /// <param name="beverages"></param>
        /// <returns></returns>
        public static BeverageConsumptionMeasurement FromExportable(this ExportableBeverageConsumptionMeasurement exportable, IEnumerable<Beverage> beverages)
        {
            var beverage = beverages.First(x => x.Name.Equals(exportable.Beverage, StringComparison.OrdinalIgnoreCase));
            return new BeverageConsumptionMeasurement
            {
                PersonId = exportable.PersonId,
                Date = exportable.Date,
                BeverageId = beverage.Id,
                Quantity = exportable.Quantity,
                Volume = exportable.Volume,
                ABV = exportable.ABV,
                Units = exportable.Units
            };
        }

        /// <summary>
        /// Return a collection of beverage consumption measurements from a collection of exportable beverage consumption measurements
        /// </summary>
        /// <param name="exportable"></param>
        /// <param name="beverages"></param>
        /// <returns></returns>
        public static IEnumerable<BeverageConsumptionMeasurement> FromExportable(
            this IEnumerable<ExportableBeverageConsumptionMeasurement> exportable,
            IEnumerable<Beverage> beverages)
        {
            var measurements = new List<BeverageConsumptionMeasurement>();

            foreach (var measurement in exportable)
            {
                measurements.Add(measurement.FromExportable(beverages));
            }

            return measurements;
        }
    }
}

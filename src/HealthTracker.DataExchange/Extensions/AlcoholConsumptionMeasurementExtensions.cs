using HealthTracker.Entities.Identity;
using HealthTracker.Entities.Measurements;
using HealthTracker.DataExchange.Entities;
using HealthTracker.Logic.Extensions;
using HealthTracker.Enumerations.Enumerations;
using HealthTracker.Enumerations.Extensions;

namespace HealthTracker.DataExchange.Extensions
{
    public static class AlcoholConsumptionMeasurementExtensions
    {
        /// <summary>
        /// Return an exportable alcohol consumption measurement from an alcohol consumption measuement
        /// </summary>
        /// <param name="measurement"></param>
        /// <param name="people"></param>
        /// <param name="beverages"></param>
        /// <returns></returns>
        public static ExportableAlcoholConsumptionMeasurement ToExportable(this AlcoholConsumptionMeasurement measurement, IEnumerable<Person> people, IEnumerable<Beverage> beverages)
        {
            var person = people.First(x => x.Id == measurement.PersonId);
            var beverage = beverages.First(x => x.Id == measurement.BeverageId);
            return new ExportableAlcoholConsumptionMeasurement
            {
                PersonId = measurement.PersonId,
                Name = $"{person.FirstNames} {person.Surname}",
                Date = measurement.Date,
                BeverageId = measurement.BeverageId,
                Beverage = beverage.Name,
                Quantity = measurement.Quantity,
                Measure = (int)measurement.Measure,
                MeasureName = measurement.Measure.ToName(),
                ABV = measurement.ABV,
                Units = measurement.Units
            };
        }

        /// <summary>
        /// Return a collection of exportable alcohol consumption measurements from a collection of alcohol consumption measurements
        /// </summary>
        /// <param name="measurements"></param>
        /// <param name="people"></param>
        /// <param name="beverages"></param>
        /// <returns></returns>
        public static IEnumerable<ExportableAlcoholConsumptionMeasurement> ToExportable(
            this IEnumerable<AlcoholConsumptionMeasurement> measurements,
            IEnumerable<Person> people,
            IEnumerable<Beverage> beverages)
        {
            var exportable = new List<ExportableAlcoholConsumptionMeasurement>();

            foreach (var measurement in measurements)
            {
                exportable.Add(measurement.ToExportable(people, beverages));
            }

            return exportable;
        }

        /// <summary>
        /// Return an alcohol consumption measurement from an exportable alcohol consumption measurement
        /// </summary>
        /// <param name="exportable"></param>
        /// <param name="beverages"></param>
        /// <returns></returns>
        public static AlcoholConsumptionMeasurement FromExportable(this ExportableAlcoholConsumptionMeasurement exportable, IEnumerable<Beverage> beverages)
        {
            var beverage = beverages.First(x => x.Name.Equals(exportable.Beverage, StringComparison.OrdinalIgnoreCase));
            return new AlcoholConsumptionMeasurement
            {
                PersonId = exportable.PersonId,
                Date = exportable.Date,
                BeverageId = beverage.Id,
                Measure = (AlcoholMeasure)exportable.Measure,
                Quantity = exportable.Quantity,
                ABV = exportable.ABV
            };
        }

        /// <summary>
        /// Return a collection of alcohol consumption measurements from a collection of exportable alcohol consumption measurements
        /// </summary>
        /// <param name="exportable"></param>
        /// <param name="beverages"></param>
        /// <returns></returns>
        public static IEnumerable<AlcoholConsumptionMeasurement> FromExportable(
            this IEnumerable<ExportableAlcoholConsumptionMeasurement> exportable,
            IEnumerable<Beverage> beverages)
        {
            var measurements = new List<AlcoholConsumptionMeasurement>();

            foreach (var measurement in exportable)
            {
                measurements.Add(measurement.FromExportable(beverages));
            }

            return measurements;
        }
    }
}

using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Food;

namespace HealthTracker.Logic.Calculations
{
    public class AlcoholUnitsCalculator : IAlcoholUnitsCalculator
    {
        internal AlcoholUnitsCalculator() { }

        /// <summary>
        /// Given an ABV % and a volume, calculate the number of units
        /// </summary>
        /// <param name="abv"></param>
        /// <param name="ml"></param>
        /// <returns></returns>
        public decimal CalculateUnits(decimal abv, decimal ml)
            => Math.Round(abv * ml / 1000M, 2, MidpointRounding.AwayFromZero);

        /// <summary>
        /// Calculate the number of units for each measurement in a collection of measurements
        /// </summary>
        /// <param name="measurements"></param>
        public void CalculateUnits(IEnumerable<BeverageConsumptionMeasurement> measurements)
        {
            if (measurements?.Count() > 0)
            {
                foreach (var measurement in measurements)
                {
                    measurement.Units = CalculateUnits(measurement.ABV, measurement.Quantity * measurement.Volume);
                }
            }
        }
    }
}
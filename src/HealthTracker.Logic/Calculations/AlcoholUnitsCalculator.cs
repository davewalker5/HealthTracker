using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Measurements;
using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.Logic.Calculations
{
    public class AlcoholUnitsCalculator : IAlcoholUnitsCalculator
    {
        private const decimal MlPerPint = 568M;

        private readonly IHealthTrackerApplicationSettings _settings;

        internal AlcoholUnitsCalculator(IHealthTrackerApplicationSettings settings)
            => _settings = settings;

        /// <summary>
        /// Calculate the volume of a quantity of a standard measure
        /// </summary>
        /// <param name="measure"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public decimal CalculateVolume(TempBeverageMeasure measure, int quantity)
            => quantity * GetVolume(measure);

        /// <summary>
        /// Return the volume associated with a specified measure
        /// </summary>
        /// <param name="measure"></param>
        /// <returns></returns>
        public decimal GetVolume(TempBeverageMeasure measure)
            => measure switch
            {
                TempBeverageMeasure.None => 0M,
                TempBeverageMeasure.Pint => MlPerPint,
                TempBeverageMeasure.HalfPint => MlPerPint / 2M,
                TempBeverageMeasure.LargeGlass => _settings.LargeGlassSize,
                TempBeverageMeasure.MediumGlass => _settings.MediumGlassSize,
                TempBeverageMeasure.SmallGlass => _settings.SmallGlassSize,
                TempBeverageMeasure.Shot => _settings.ShotSize,
                _ => 0M,
            };

        /// <summary>
        /// Given an ABV % and a volume, calculate the number of units
        /// </summary>
        /// <param name="abv"></param>
        /// <param name="ml"></param>
        /// <returns></returns>
        public decimal CalculateUnits(decimal abv, decimal ml)
            => Math.Round(abv * ml / 1000M, 2, MidpointRounding.AwayFromZero);

        /// <summary>
        /// Calculate the number of units in a shot
        /// </summary>
        /// <param name="abv"></param>
        /// <returns></returns>
        public decimal UnitsPerShot(decimal abv)
            => CalculateUnits(abv, _settings.ShotSize);

        /// <summary>
        /// Calculate the number of units in a pint
        /// </summary>
        /// <param name="abv"></param>
        /// <returns></returns>
        public decimal UnitsPerPint(decimal abv)
            => CalculateUnits(abv, MlPerPint);

        /// <summary>
        /// Calculate the number of units in half a pint
        /// </summary>
        /// <param name="abv"></param>
        /// <returns></returns>
        public decimal UnitsPerHalfPint(decimal abv)
            => CalculateUnits(abv, MlPerPint / 2M);

        /// <summary>
        /// Calculate the number of units in a small glass
        /// </summary>
        /// <param name="abv"></param>
        /// <returns></returns>
        public decimal UnitsPerSmallGlass(decimal abv)
            => CalculateUnits(abv, _settings.SmallGlassSize);

        /// <summary>
        /// Calculate the number of units in a medium glass
        /// </summary>
        /// <param name="abv"></param>
        /// <returns></returns>
        public decimal UnitsPerMediumGlass(decimal abv)
            => CalculateUnits(abv, _settings.MediumGlassSize);

        /// <summary>
        /// Calculate the number of units in a large glass
        /// </summary>
        /// <param name="abv"></param>
        /// <returns></returns>
        public decimal UnitsPerLargeGlass(decimal abv)
            => CalculateUnits(abv, _settings.LargeGlassSize);

        /// <summary>
        /// Calculate the number of units for each measurement in a collection of measurements
        /// </summary>
        /// <param name="measurements"></param>
        public void CalculateUnits(IEnumerable<BeverageConsumptionMeasurement> measurements)
        {
            // Iterate over the records
            if (measurements?.Count() > 0)
            {
                foreach (var measurement in measurements)
                {
                    // Calculate the units for a single measure
                    var unitsPerMeasure = measurement.Measure switch
                    {
                        TempBeverageMeasure.Pint => UnitsPerPint(measurement.ABV),
                        TempBeverageMeasure.LargeGlass => UnitsPerLargeGlass(measurement.ABV),
                        TempBeverageMeasure.MediumGlass => UnitsPerMediumGlass(measurement.ABV),
                        TempBeverageMeasure.SmallGlass => UnitsPerSmallGlass(measurement.ABV),
                        TempBeverageMeasure.Shot => UnitsPerShot(measurement.ABV),
                        _ => 0M,
                    };

                    // Calculate the total units for the record
                    measurement.Units = measurement.Quantity * unitsPerMeasure;
                }
            }
        }
    }
}
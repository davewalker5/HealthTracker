using HealthTracker.Configuration.Interfaces;
using HealthTracker.Entities.Interfaces;

namespace HealthTracker.Logic.Calculations
{
    public class AlcoholUnitsCalculator : IAlcoholUnitsCalculator
    {
        private const decimal MlPerPint = 568M;

        private readonly IHealthTrackerApplicationSettings _settings;

        internal AlcoholUnitsCalculator(IHealthTrackerApplicationSettings settings)
            => _settings = settings;

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
    }
}
using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.Enumerations.Extensions
{
    public static class BeverageMeasureExtensions
    {
        /// <summary>
        /// Convert a beverage measure to a descriptive string
        /// </summary>
        /// <param name="units"></param>
        /// <returns></returns>
        public static string ToName(this TempBeverageMeasure measure)
        {
            return measure switch
            {
                TempBeverageMeasure.None => "",
                TempBeverageMeasure.Pint => "Pint",
                TempBeverageMeasure.HalfPint => "Half Pint",
                TempBeverageMeasure.LargeGlass => "Large Glass",
                TempBeverageMeasure.MediumGlass => "Medium Glass",
                TempBeverageMeasure.SmallGlass => "Small Glass",
                TempBeverageMeasure.Shot => "Shot",
                _ => "",
            };
        }
    }
}
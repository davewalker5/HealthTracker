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
        public static string ToName(this BeverageMeasure measure)
        {
            return measure switch
            {
                BeverageMeasure.None => "",
                BeverageMeasure.Pint => "Pint",
                BeverageMeasure.HalfPint => "Half Pint",
                BeverageMeasure.LargeGlass => "Large Glass",
                BeverageMeasure.MediumGlass => "Medium Glass",
                BeverageMeasure.SmallGlass => "Small Glass",
                BeverageMeasure.Shot => "Shot",
                _ => "",
            };
        }
    }
}
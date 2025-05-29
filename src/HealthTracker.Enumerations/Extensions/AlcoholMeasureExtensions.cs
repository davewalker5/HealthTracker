
using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.Enumerations.Extensions
{
    public static class AlcoholMeasureExtensions
    {
        /// <summary>
        /// Convert an alcohol measure to a descriptive string
        /// </summary>
        /// <param name="units"></param>
        /// <returns></returns>
        public static string ToName(this AlcoholMeasure measure)
        {
            return measure switch
            {
                AlcoholMeasure.None => "",
                AlcoholMeasure.Pint => "Pint",
                AlcoholMeasure.LargeGlass => "Large Glass",
                AlcoholMeasure.MediumGlass => "Medium Glass",
                AlcoholMeasure.SmallGlass => "Small Glass",
                AlcoholMeasure.Shot => "Shot",
                _ => "",
            };
        }
    }
}
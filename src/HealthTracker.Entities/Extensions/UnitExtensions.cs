using HealthTracker.Entities.Measurements;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public static class UnitExtensions
{
    /// <summary>
    /// Convert blood presure measurement units to a descriptive string
    /// </summary>
    /// <param name="units"></param>
    /// <returns></returns>
    public static string ToDescription(this BloodPressureUnit units)
    {
        return units switch
        {
            BloodPressureUnit.MillimetresOfMercury => "mmHg",
            _ => "",
        };
    }

    /// <summary>
    /// Convert cholesterol measurement units to a descriptive string
    /// </summary>
    /// <param name="units"></param>
    /// <returns></returns>
    public static string ToDescription(this CholesterolUnit units)
    {
        return units switch
        {
            CholesterolUnit.MillimolesPerLitre => "mmol/L",
            _ => "",
        };
    }

    /// <summary>
    /// Convert distance measurement units to a descriptive string
    /// </summary>
    /// <param name="units"></param>
    /// <returns></returns>
    public static string ToDescription(this DistanceUnit units)
    {
        return units switch
        {
            DistanceUnit.Metres => "m",
            DistanceUnit.Kilometres => "km",
            DistanceUnit.Miles => "miles",
            _ => "",
        };
    }

    /// <summary>
    /// Convert HR measurement units to a descriptive string
    /// </summary>
    /// <param name="units"></param>
    /// <returns></returns>
    public static string ToDescription(this HeartRateUnit units)
    {
        return units switch
        {
            HeartRateUnit.BeatsPerMinute => "bpm",
            _ => "",
        };
    }

    /// <summary>
    /// Convert height measurement units to a descriptive string
    /// </summary>
    /// <param name="units"></param>
    /// <returns></returns>
    public static string ToDescription(this HeightUnit units)
    {
        return units switch
        {
            HeightUnit.Centimetres => "cm",
            HeightUnit.Metres => "m",
            HeightUnit.Feet => "feet",
            _ => "",
        };
    }

    /// <summary>
    /// Convert weight measurement units to a descriptive string
    /// </summary>
    /// <param name="units"></param>
    /// <returns></returns>
    public static string ToDescription(this WeightUnit units)
    {
        return units switch
        {
            WeightUnit.Kilograms => "kg",
            WeightUnit.Pounds => "lbs",
            _ => "",
        };
    }
}
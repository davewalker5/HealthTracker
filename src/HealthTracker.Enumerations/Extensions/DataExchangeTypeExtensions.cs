using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.Enumerations.Extensions
{
    public static class DataExchangeTypeExtensions
    {
        /// <summary>
        /// Convert a data exchange type to a descriptive string
        /// </summary>
        /// <param name="units"></param>
        /// <returns></returns>
        public static string ToName(this DataExchangeType type)
        {
            return type switch
            {
                DataExchangeType.None => "",
                DataExchangeType.SPO2 => "% SPO2",
                DataExchangeType.BloodPressure => "Blood Pressure",
                DataExchangeType.Exercise => "Exercise",
                DataExchangeType.FoodItems => "Food Items",
                DataExchangeType.Glucose => "Glucose",
                DataExchangeType.Weight => "Weight",
                DataExchangeType.BeverageConsumption => "Beverage Consumption",
                DataExchangeType.Meals => "Meals",
                DataExchangeType.MealConsumption => "Meal Consumption",
                DataExchangeType.MealFoodItem => "Meal/Food Item Relationships",
                DataExchangeType.PlannedMeals => "Scheduled Meals",
                _ => "",
            };
        }
    }
}
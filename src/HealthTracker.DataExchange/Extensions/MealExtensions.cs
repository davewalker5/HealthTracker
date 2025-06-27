using HealthTracker.Entities.Food;
using HealthTracker.DataExchange.Entities;

namespace HealthTracker.DataExchange.Extensions
{
    public static class MealExtensions
    {
        /// <summary>
        /// Return an exportable meal from a meal
        /// </summary>
        /// <param name="meal"></param>
        /// <returns></returns>
        public static ExportableMeal ToExportable(this Meal meal)
            => new()
            {
                Name = meal.Name,
                Portions = meal.Portions,
                Calories = meal.NutritionalValue?.Calories,
                Fat = meal.NutritionalValue?.Fat,
                SaturatedFat = meal.NutritionalValue?.SaturatedFat,
                Protein = meal.NutritionalValue?.Protein,
                Carbohydrates = meal.NutritionalValue?.Carbohydrates,
                Sugar = meal.NutritionalValue?.Sugar,
                Fibre = meal.NutritionalValue?.Fibre
            };

        /// <summary>
        /// Return a collection of exportable meals from a collection of meals
        /// </summary>
        /// <param name="meals"></param>
        /// <returns></returns>
        public static IEnumerable<ExportableMeal> ToExportable(this IEnumerable<Meal> meals)
        {
            var exportable = new List<ExportableMeal>();

            foreach (var meal in meals)
            {
                exportable.Add(meal.ToExportable());
            }

            return exportable;
        }

        /// <summary>
        /// Return a meal from an exportable meal
        /// </summary>
        /// <param name="meal"></param>
        /// <returns></returns>
        public static Meal FromExportable(this ExportableMeal meal)
            => new ()
            {
                Name = meal.Name,
                Portions = meal.Portions,
                NutritionalValue = new NutritionalValue
                {
                    Calories = meal.Calories,
                    Fat = meal.Fat,
                    SaturatedFat = meal.SaturatedFat,
                    Protein = meal.Protein,
                    Carbohydrates = meal.Carbohydrates,
                    Sugar = meal.Sugar,
                    Fibre = meal.Fibre
                }
            };

        /// <summary>
        /// Return a collection of meals from a collection of exportable meals
        /// </summary>
        /// <param name="exportable"></param>
        /// <returns></returns>
        public static IEnumerable<Meal> FromExportable(this IEnumerable<ExportableMeal> exportable)
        {
            var meals = new List<Meal>();

            foreach (var meal in exportable)
            {
                meals.Add(meal.FromExportable());
            }

            return meals;
        }
    }
}

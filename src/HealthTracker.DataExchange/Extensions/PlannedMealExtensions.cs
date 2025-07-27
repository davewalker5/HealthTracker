using HealthTracker.Entities.Food;
using HealthTracker.DataExchange.Entities;
using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.DataExchange.Extensions
{
    public static class PlannedMealExtensions
    {
        /// <summary>
        /// Return an exportable planned meal from a planned meal
        /// </summary>
        /// <param name="plannedMeal"></param>
        /// <returns></returns>
        public static ExportablePlannedMeal ToExportable(this PlannedMeal plannedMeal)
            => new()
            {
                MealType = plannedMeal.MealType.ToString(),
                Date = plannedMeal.Date,
                Meal = plannedMeal.Meal.Name
            };

        /// <summary>
        /// Return a collection of exportable planned meals from a collection of planned meals
        /// </summary>
        /// <param name="plannedMeals"></param>
        /// <returns></returns>
        public static IEnumerable<ExportablePlannedMeal> ToExportable(this IEnumerable<PlannedMeal> plannedMeals)
        {
            var exportable = new List<ExportablePlannedMeal>();

            foreach (var plannedMeal in plannedMeals)
            {
                exportable.Add(plannedMeal.ToExportable());
            }

            return exportable;
        }

        /// <summary>
        /// Return a planned meal from an exportable planned meal
        /// </summary>
        /// <param name="exportable"></param>
        /// <param name="meals"></param>
        /// <returns></returns>
        public static PlannedMeal FromExportable(this ExportablePlannedMeal exportable, IEnumerable<Meal> meals)
        {
            var meal = meals.First(x => x.Name == exportable.Meal);
            return new()
            {
                MealType = (MealType)Enum.Parse(typeof(MealType), exportable.MealType),
                Date = exportable.Date,
                MealId = meal.Id,
                Meal = meal
            };
        }

        /// <summary>
        /// Return a collection of planned meals from a collection of exportable planned meals
        /// </summary>
        /// <param name="exportable"></param>
        /// <param name="meals"></param>
        /// <returns></returns>
        public static IEnumerable<PlannedMeal> FromExportable(this IEnumerable<ExportablePlannedMeal> exportable, IEnumerable<Meal> meals)
        {
            var plannedMeals = new List<PlannedMeal>();

            foreach (var plannedMeal in exportable)
            {
                plannedMeals.Add(plannedMeal.FromExportable(meals));
            }

            return plannedMeals;
        }
    }
}

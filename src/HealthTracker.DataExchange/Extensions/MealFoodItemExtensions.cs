using HealthTracker.Entities.Food;
using HealthTracker.DataExchange.Entities;

namespace HealthTracker.DataExchange.Extensions
{
    public static class MealFoodItemExtensions
    {
        /// <summary>
        /// Return an exportable meal/food item relationship from a single relationship
        /// </summary>
        /// <param name="relationship"></param>
        /// <param name="meals"></param>
        /// <param name="foodItems"></param>
        /// <returns></returns>
        public static ExportableMealFoodItem ToExportable(this MealFoodItem relationship, IEnumerable<Meal> meals, IEnumerable<FoodItem> foodItems)
            => new()
            {
                Meal = meals.First(x => x.Id == relationship.MealId).Name,
                FoodItem = foodItems.First(x => x.Id == relationship.FoodItemId).Name,
                Quantity = relationship.Quantity,
                Calories = relationship.NutritionalValue?.Calories,
                Fat = relationship.NutritionalValue?.Fat,
                SaturatedFat = relationship.NutritionalValue?.SaturatedFat,
                Protein = relationship.NutritionalValue?.Protein,
                Carbohydrates = relationship.NutritionalValue?.Carbohydrates,
                Sugar = relationship.NutritionalValue?.Sugar,
                Fibre = relationship.NutritionalValue?.Fibre
            };

        /// <summary>
        /// Return a collection of exportable meal/food item relationships from a collection of relationships
        /// </summary>
        /// <param name="relationships"></param>
        /// <param name="meals"></param>
        /// <param name="foodItems"></param>
        /// <returns></returns>
        public static IEnumerable<ExportableMealFoodItem> ToExportable(
            this IEnumerable<MealFoodItem> relationships,
            IEnumerable<Meal> meals,
            IEnumerable<FoodItem> foodItems)
        {
            var exportable = new List<ExportableMealFoodItem>();

            foreach (var relationship in relationships)
            {
                exportable.Add(relationship.ToExportable(meals, foodItems));
            }

            return exportable;
        }

        /// <summary>
        /// Return a meal/food item relationship from a single exportable relationship
        /// </summary>
        /// <param name="exportable"></param>
        /// <param name="meals"></param>
        /// <param name="foodItems"></param>
        /// <returns></returns>
        public static MealFoodItem FromExportable(this ExportableMealFoodItem exportable, IEnumerable<Meal> meals, IEnumerable<FoodItem> foodItems)
            => new()
            {
                MealId = meals.First(x => x.Name == exportable.Meal).Id,
                FoodItemId = foodItems.First(x => x.Name == exportable.FoodItem).Id,
                Quantity = exportable.Quantity,
                NutritionalValue = new NutritionalValue
                {
                    Calories = exportable.Calories,
                    Fat = exportable.Fat,
                    SaturatedFat = exportable.SaturatedFat,
                    Protein = exportable.Protein,
                    Carbohydrates = exportable.Carbohydrates,
                    Sugar = exportable.Sugar,
                    Fibre = exportable.Fibre
                }
            };

        /// <summary>
        /// Return a collection of meal/food item relationships from a collection of exportable relationships
        /// </summary>
        /// <param name="exportable"></param>
        /// <param name="meals"></param>
        /// <param name="foodItems"></param>
        /// <returns></returns>
        public static IEnumerable<MealFoodItem> FromExportable(
            this IEnumerable<ExportableMealFoodItem> exportable,
            IEnumerable<Meal> meals,
            IEnumerable<FoodItem> foodItems)
        {
            var relationships = new List<MealFoodItem>();

            foreach (var relationship in exportable)
            {
                relationships.Add(relationship.FromExportable(meals, foodItems));
            }

            return relationships;
        }
    }
}

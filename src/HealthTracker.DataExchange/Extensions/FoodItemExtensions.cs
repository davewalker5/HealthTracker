using HealthTracker.Entities.Food;
using HealthTracker.DataExchange.Entities;

namespace HealthTracker.DataExchange.Extensions
{
    public static class FoodItemExtensions
    {
        /// <summary>
        /// Return an exportable food item from a food item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static ExportableFoodItem ToExportable(this FoodItem item)
            => new()
            {
                Name = item.Name,
                FoodCategory = item.FoodCategory.Name,
                Portion = item.Portion,
                Calories = item.NutritionalValue?.Calories,
                Fat = item.NutritionalValue?.Fat,
                SaturatedFat = item.NutritionalValue?.SaturatedFat,
                Protein = item.NutritionalValue?.Protein,
                Carbohydrates = item.NutritionalValue?.Carbohydrates,
                Sugar = item.NutritionalValue?.Sugar,
                Fibre = item.NutritionalValue?.Fibre
            };

        /// <summary>
        /// Return a collection of exportable food items from a collection of food items
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static IEnumerable<ExportableFoodItem> ToExportable(this IEnumerable<FoodItem> items)
        {
            var exportable = new List<ExportableFoodItem>();

            foreach (var item in items)
            {
                exportable.Add(item.ToExportable());
            }

            return exportable;
        }

        /// <summary>
        /// Return a food item from an exportable food item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="categories"></param>
        /// <returns></returns>
        public static FoodItem FromExportable(this ExportableFoodItem item, IEnumerable<FoodCategory> categories)
        {
            var category = categories.First(x => x.Name == item.FoodCategory);
            return new()
            {
                Name = item.Name,
                FoodCategory = category,
                FoodCategoryId = category.Id,
                Portion = item.Portion,
                NutritionalValue = new NutritionalValue
                {
                    Calories = item.Calories,
                    Fat = item.Fat,
                    SaturatedFat = item.SaturatedFat,
                    Protein = item.Protein,
                    Carbohydrates = item.Carbohydrates,
                    Sugar = item.Sugar,
                    Fibre = item.Fibre
                }
            };
        }

        /// <summary>
        /// Return a collection of food items from a collection of exportable food items
        /// </summary>
        /// <param name="exportable"></param>
        /// <param name="categories"></param>
        /// <returns></returns>
        public static IEnumerable<FoodItem> FromExportable(this IEnumerable<ExportableFoodItem> exportable, IEnumerable<FoodCategory> categories)
        {
            var items = new List<FoodItem>();

            foreach (var item in exportable)
            {
                items.Add(item.FromExportable(categories));
            }

            return items;
        }
    }
}

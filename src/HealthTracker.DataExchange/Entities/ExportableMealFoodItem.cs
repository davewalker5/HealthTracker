using HealthTracker.DataExchange.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportableMealFoodItem : ExportableEntityBase
    {
        public const string CsvRecordPattern = @"^"".*"","".*"".?$";

        [Export("Meal", 1)]
        public string Meal { get; set; }

        [Export("Food Item", 2)]
        public string FoodItem { get; set; }

        public static ExportableMealFoodItem FromCsv(string record)
        {
            string[] words = record.Split(["\",\""], StringSplitOptions.None);
            return new ExportableMealFoodItem
            {
                Meal = words[0].Replace("\"", "").Trim(),
                FoodItem = words[1].Replace("\"", "").Trim()
            };
        }
    }
}

using HealthTracker.DataExchange.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportableMealFoodItem : ExportableEntityBase
    {
        public const string CsvRecordPattern = @"^"".*"","".*""(,""(?:[0-9.]+)?""){8}.?$";

        [Export("Meal", 1)]
        public string Meal { get; set; }

        [Export("Food Item", 2)]
        public string FoodItem { get; set; }

        [Export("Quantity", 3)]
        public decimal Quantity { get; set; }

        [Export("Calories", 4)]
        public decimal? Calories { get; set; }

        [Export("Fat", 5)]
        public decimal? Fat { get; set; }

        [Export("Saturated Fat", 6)]
        public decimal? SaturatedFat { get; set; }

        [Export("Protein", 7)]
        public decimal? Protein { get; set; }

        [Export("Carbohydrates", 8)]
        public decimal? Carbohydrates { get; set; }

        [Export("Sugar", 9)]
        public decimal? Sugar { get; set; }

        [Export("Fibre", 10)]
        public decimal? Fibre { get; set; }

        public static ExportableMealFoodItem FromCsv(string record)
        {
            string[] words = record.Split(["\",\""], StringSplitOptions.None);
            return new ExportableMealFoodItem
            {
                Meal = words[0].Replace("\"", "").Trim(),
                FoodItem = words[1].Replace("\"", "").Trim(),
                Quantity = decimal.Parse(words[2].Replace("\"", "").Trim()),
                Calories = ExtractDecimalValue(words[3]),
                Fat = ExtractDecimalValue(words[4]),
                SaturatedFat = ExtractDecimalValue(words[5]),
                Protein = ExtractDecimalValue(words[6]),
                Carbohydrates = ExtractDecimalValue(words[7]),
                Sugar = ExtractDecimalValue(words[8]),
                Fibre = ExtractDecimalValue(words[9])
            };
        }

        private static decimal? ExtractDecimalValue(string representation)
        {
            var trimmed = representation?.Replace("\"", "").Trim();
            return string.IsNullOrEmpty(trimmed) ? null : decimal.Parse(trimmed);
        }
    }
}

using HealthTracker.DataExchange.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportableMealFoodItem : ExportableEntityBase
    {
        public const string CsvRecordPattern = @"^"".*"","".*"","".*""(,""(?:[0-9.]+)?""){8}.?$";

        [Export("Meal", 1)]
        public string Meal { get; set; }

        [Export("Source", 2)]
        public string Source { get; set; }

        [Export("Food Item", 3)]
        public string FoodItem { get; set; }

        [Export("Quantity", 4)]
        public decimal Quantity { get; set; }

        [Export("Calories", 5)]
        public decimal? Calories { get; set; }

        [Export("Fat", 6)]
        public decimal? Fat { get; set; }

        [Export("Saturated Fat", 7)]
        public decimal? SaturatedFat { get; set; }

        [Export("Protein", 8)]
        public decimal? Protein { get; set; }

        [Export("Carbohydrates", 9)]
        public decimal? Carbohydrates { get; set; }

        [Export("Sugar", 10)]
        public decimal? Sugar { get; set; }

        [Export("Fibre", 11)]
        public decimal? Fibre { get; set; }

        public static ExportableMealFoodItem FromCsv(string record)
        {
            string[] words = record.Split(["\",\""], StringSplitOptions.None);
            return new ExportableMealFoodItem
            {
                Meal = words[0].Replace("\"", "").Trim(),
                Source = words[1].Replace("\"", "").Trim(),
                FoodItem = words[2].Replace("\"", "").Trim(),
                Quantity = decimal.Parse(words[3].Replace("\"", "").Trim()),
                Calories = ExtractDecimalValue(words[4]),
                Fat = ExtractDecimalValue(words[5]),
                SaturatedFat = ExtractDecimalValue(words[6]),
                Protein = ExtractDecimalValue(words[7]),
                Carbohydrates = ExtractDecimalValue(words[8]),
                Sugar = ExtractDecimalValue(words[9]),
                Fibre = ExtractDecimalValue(words[10])
            };
        }

        private static decimal? ExtractDecimalValue(string representation)
        {
            var trimmed = representation?.Replace("\"", "").Trim();
            return string.IsNullOrEmpty(trimmed) ? null : decimal.Parse(trimmed);
        }
    }
}

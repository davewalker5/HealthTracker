using HealthTracker.DataExchange.Attributes;
using System;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportableFoodItem
    {
        public const string CsvRecordPattern = @"^"".*"","".*""(,""(?:[0-9.]+)?""){8}.?$";

        [Export("Name", 1)]
        public string Name { get; set; }

        [Export("Category", 2)]
        public string FoodCategory { get; set; }

        [Export("Portion", 3)]
        public decimal Portion { get; set; }

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

        public static ExportableFoodItem FromCsv(string record)
        {
            string[] words = record.Split(["\",\""], StringSplitOptions.None);
            return new ExportableFoodItem
            {
                Name = words[0].Replace("\"", "").Trim(),
                FoodCategory = words[1].Replace("\"", "").Trim(),
                Portion = decimal.Parse(words[2].Replace("\"", "").Trim()),
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
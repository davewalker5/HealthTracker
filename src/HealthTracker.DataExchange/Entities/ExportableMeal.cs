using HealthTracker.DataExchange.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportableMeal
    {
        public const string CsvRecordPattern = @"^"".*"","".*"","".*"",""[0-9]+""(,""(?:[0-9.]+)?""){7}.?$";

        [Export("Name", 1)]
        public string Name { get; set; }

        [Export("Source", 2)]
        public string FoodSource { get; set; }

        [Export("Reference", 3)]
        public string Reference { get; set; }

        [Export("Portions", 4)]
        public int Portions { get; set; }

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

        public static ExportableMeal FromCsv(string record)
        {
            string[] words = record.Split(["\",\""], StringSplitOptions.None);
            return new ExportableMeal
            {
                Name = words[0].Replace("\"", "").Trim(),
                FoodSource = words[1].Replace("\"", "").Trim(),
                Reference = words[2].Replace("\"", "").Trim(),
                Portions = int.Parse(words[3].Replace("\"", "").Trim()),
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
using HealthTracker.DataExchange.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportableMeal
    {
        public const string CsvRecordPattern = @"^"".*"",""[0-9]+""(,""(?:[0-9.]+)?""){7}.?$";

        [Export("Name", 1)]
        public string Name { get; set; }

        [Export("Portions", 3)]
        public int Portions { get; set; }

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

        public static ExportableMeal FromCsv(string record)
        {
            string[] words = record.Split(["\",\""], StringSplitOptions.None);
            return new ExportableMeal
            {
                Name = words[0].Replace("\"", "").Trim(),
                Portions = int.Parse(words[1].Replace("\"", "").Trim()),
                Calories = ExtractDecimalValue(words[2]),
                Fat = ExtractDecimalValue(words[3]),
                SaturatedFat = ExtractDecimalValue(words[4]),
                Protein = ExtractDecimalValue(words[5]),
                Carbohydrates = ExtractDecimalValue(words[6]),
                Sugar = ExtractDecimalValue(words[7]),
                Fibre = ExtractDecimalValue(words[8])
            };
        }

        private static decimal? ExtractDecimalValue(string representation)
        {
            var trimmed = representation?.Replace("\"", "").Trim();
            return string.IsNullOrEmpty(trimmed) ? null : decimal.Parse(trimmed);
        }
    }
}
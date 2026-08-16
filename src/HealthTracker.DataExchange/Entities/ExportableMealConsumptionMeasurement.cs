using HealthTracker.DataExchange.Attributes;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportableMealConsumptionMeasurement : ExportableMeasurementBase
    {
        public const string CsvRecordPattern = @"^""[0-9]+"","".*"",""[0-9]+-[A-Za-z]+-[0-9]+ [0-9]+:[0-9]+:[0-9]+"",""[0-9]+"","".*"","".*""(,""(?:[0-9.]+)?""){8}.?$";

        [Export("Meal Id", 4)]
        public int MealId { get; set; }

        [Export("Meal", 5)]
        public string Meal { get; set; }

        [Export("Source", 6)]
        public string Source { get; set; }

        [Export("Quantity", 7)]
        public decimal Quantity { get; set; }

        [Export("Calories", 8)]
        public decimal? Calories { get; set; }

        [Export("Fat", 9)]
        public decimal? Fat { get; set; }

        [Export("Saturated Fat", 10)]
        public decimal? SaturatedFat { get; set; }

        [Export("Protein", 11)]
        public decimal? Protein { get; set; }

        [Export("Carbohydrates", 12)]
        public decimal? Carbohydrates { get; set; }

        [Export("Sugar", 13)]
        public decimal? Sugar { get; set; }

        [Export("Fibre", 14)]
        public decimal? Fibre { get; set; }

        public static ExportableMealConsumptionMeasurement FromCsv(string record)
        {
            string[] words = record.Split(["\",\""], StringSplitOptions.None);
            return new ExportableMealConsumptionMeasurement
            {
                PersonId = int.Parse(words[0].Replace("\"", "").Trim()),
                Name = words[1].Replace("\"", "").Trim(),
                Date = DateTime.ParseExact(words[2].Replace("\"", "").Trim(), TimestampFormat, CultureInfo.CurrentCulture),
                MealId = int.Parse(words[3].Replace("\"", "").Trim()),
                Meal = words[4].Replace("\"", "").Trim(),
                Source = words[5].Replace("\"", "").Trim(),
                Quantity = decimal.Parse(words[6].Replace("\"", "").Trim()),
                Calories = ExtractDecimalValue(words[7]),
                Fat = ExtractDecimalValue(words[8]),
                SaturatedFat = ExtractDecimalValue(words[9]),
                Protein = ExtractDecimalValue(words[10]),
                Carbohydrates = ExtractDecimalValue(words[11]),
                Sugar = ExtractDecimalValue(words[12]),
                Fibre = ExtractDecimalValue(words[13])
            };
        }

        private static decimal? ExtractDecimalValue(string representation)
        {
            var trimmed = representation?.Replace("\"", "").Trim();
            return string.IsNullOrEmpty(trimmed) ? null : decimal.Parse(trimmed);
        }
    }
}

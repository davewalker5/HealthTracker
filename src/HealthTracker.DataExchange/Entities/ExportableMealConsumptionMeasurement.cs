using HealthTracker.DataExchange.Attributes;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportableMealConsumptionMeasurement : ExportableMeasurementBase
    {
        public const string CsvRecordPattern = @"^""[0-9]+"","".*"",""[0-9]+-[A-Za-z]+-[0-9]+ [0-9]+:[0-9]+:[0-9]+"",""[0-9]+"","".*""(,""(?:[0-9.]+)?""){8}.?$";

        [Export("Meal Id", 4)]
        public int MealId { get; set; }

        [Export("Meal", 5)]
        public string Meal { get; set; }

        [Export("Quantity", 6)]
        public decimal Quantity { get; set; }

        [Export("Calories", 7)]
        public decimal? Calories { get; set; }

        [Export("Fat", 8)]
        public decimal? Fat { get; set; }

        [Export("Saturated Fat", 9)]
        public decimal? SaturatedFat { get; set; }

        [Export("Protein", 10)]
        public decimal? Protein { get; set; }

        [Export("Carbohydrates", 11)]
        public decimal? Carbohydrates { get; set; }

        [Export("Sugar", 12)]
        public decimal? Sugar { get; set; }

        [Export("Fibre", 13)]
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
                Quantity = decimal.Parse(words[5].Replace("\"", "").Trim()),
                Calories = ExtractDecimalValue(words[6]),
                Fat = ExtractDecimalValue(words[7]),
                SaturatedFat = ExtractDecimalValue(words[8]),
                Protein = ExtractDecimalValue(words[9]),
                Carbohydrates = ExtractDecimalValue(words[10]),
                Sugar = ExtractDecimalValue(words[11]),
                Fibre = ExtractDecimalValue(words[12])
            };
        }

        private static decimal? ExtractDecimalValue(string representation)
        {
            var trimmed = representation?.Replace("\"", "").Trim();
            return string.IsNullOrEmpty(trimmed) ? null : decimal.Parse(trimmed);
        }
    }
}

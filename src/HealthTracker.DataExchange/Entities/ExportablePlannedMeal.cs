using HealthTracker.DataExchange.Attributes;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace HealthTracker.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportablePlannedMeal : ExportableMeasurementBase
    {
        public const string CsvRecordPattern = @"^""[0-9]+"","".*"",""[0-9]+-[A-Za-z]+-[0-9]+ [0-9]+:[0-9]+:[0-9]+"","".*"","".*"","".*"","".*"".?$";

        [Export("Meal Type", 4)]
        public string MealType { get; set; }

        [Export("Meal", 5)]
        public string Meal { get; set; }

        [Export("Source", 6)]
        public string Source { get; set; }

        [Export("Reference", 7)]
        public string Reference { get; set; }

        public static ExportablePlannedMeal FromCsv(string record)
        {
            string[] words = record.Split(["\",\""], StringSplitOptions.None);
            return new ExportablePlannedMeal
            {
                PersonId = int.Parse(words[0].Replace("\"", "").Trim()),
                Name = words[1].Replace("\"", "").Trim(),
                Date = DateTime.ParseExact(words[2].Replace("\"", "").Trim(), TimestampFormat, CultureInfo.CurrentCulture),
                MealType = words[3].Replace("\"", "").Trim(),
                Meal = words[4].Replace("\"", "").Trim(),
                Source = words[5].Replace("\"", "").Trim(),
                Reference = words[6].Replace("\"", "").Trim(),
            };
        }
    }
}
using HealthTracker.DataExchange.Attributes;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace HealthTracker.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportablePlannedMeal : ExportableEntityBase
    {
        public const string CsvRecordPattern = @"^"".*"",""[0-9]+\/[0-9]+\/[0-9]+"","".*"".?$";

        [Export("Meal Type", 1)]
        public string MealType { get; set; }

        [Export("Date", 2)]
        public DateTime Date { get; set; }

        [Export("Meal", 3)]
        public string Meal { get; set; }

        public static ExportablePlannedMeal FromCsv(string record)
        {
            string[] words = record.Split(["\",\""], StringSplitOptions.None);
            return new ExportablePlannedMeal
            {
                MealType = words[0].Replace("\"", "").Trim(),
                Date = DateTime.ParseExact(words[1].Replace("\"", "").Trim(), DateTimeFormat, CultureInfo.CurrentCulture),
                Meal = words[2].Replace("\"", "").Trim(),
            };
        }
    }
}
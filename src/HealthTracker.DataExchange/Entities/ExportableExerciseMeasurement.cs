using HealthTracker.DataExchange.Attributes;
using System;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportableExerciseMeasurement : ExportableMeasurementBase
    {
        public const string CsvRecordPattern = @"^""[0-9]+"","".*"",""[0-9]+\/[0-9]+\/[0-9]+"","".*"",""[0-9]+:[0-9]+:[0-9]+"",""[0-9\.]+"",""[0-9]+"",""[0-9]+"",""[0-9]+"".?$";

        [Export("ActivityType", 4)]
        public string ActivityType { get; set; }

        [Export("Duration", 5)]
        public string Duration { get; set; }

        [Export("Distance", 6)]
        public decimal? Distance { get; set; }

        [Export("Calories", 7)]
        public int Calories { get; set; }

        [Export("MinimumHeartRate", 8)]
        public int MinimumHeartRate { get; set; }

        [Export("MaximumHeartRate", 9)]
        public int MaximumHeartRate { get; set; }

        public static ExportableExerciseMeasurement FromCsv(string record)
        {
            string[] words = record.Split(["\",\""], StringSplitOptions.None);
            return new ExportableExerciseMeasurement
            {
                PersonId = int.Parse(words[0].Replace("\"", "").Trim()),
                Name = words[1].Replace("\"", "").Trim(),
                Date = DateTime.ParseExact(words[2].Replace("\"", "").Trim(), DateTimeFormat, CultureInfo.CurrentCulture),
                ActivityType = words[3].Replace("\"", "").Trim(),
                Duration = words[4].Replace("\"", "").Trim(),
                Distance = !string.IsNullOrEmpty(words[5]) ? decimal.Parse(words[5]) : 0,
                Calories = int.Parse(words[6].Replace("\"", "").Trim()),
                MinimumHeartRate = int.Parse(words[7].Replace("\"", "").Trim()),
                MaximumHeartRate = int.Parse(words[8].Replace("\"", "").Trim())
            };
        }
    }
}

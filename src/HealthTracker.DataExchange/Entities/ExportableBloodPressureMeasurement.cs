using HealthTracker.DataExchange.Attributes;
using System;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportableBloodPressureMeasurement : ExportableMeasurementBase
    {
        public const string CsvRecordPattern = @"^""[0-9]+"","".*"",""[0-9]+\/[0-9]+\/[0-9]+"",""[0-9]+"",""[0-9]+"","".*"".?$";

        [Export("Systolic", 4)]
        public int Systolic { get; set; }

        [Export("Diastolic", 5)]
        public int Diastolic { get; set; }

        [Export("Assessment", 6)]
        public string Assessment { get; set; }

        public static ExportableBloodPressureMeasurement FromCsv(string record)
        {
            string[] words = record.Split(["\",\""], StringSplitOptions.None);
            return new ExportableBloodPressureMeasurement
            {
                PersonId = int.Parse(words[0].Replace("\"", "").Trim()),
                Name = words[1].Replace("\"", "").Trim(),
                Date = DateTime.ParseExact(words[2].Replace("\"", "").Trim(), DateTimeFormat, CultureInfo.CurrentCulture),
                Systolic = int.Parse(words[3].Replace("\"", "").Trim()),
                Diastolic = int.Parse(words[4].Replace("\"", "").Trim()),
                Assessment = words[5].Replace("\"", "").Trim()
            };
        }
    }
}

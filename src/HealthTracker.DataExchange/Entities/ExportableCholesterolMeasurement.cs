using HealthTracker.DataExchange.Attributes;
using System;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportableCholesterolMeasurement : ExportableMeasurementBase
    {
        public const string CsvRecordPattern = @"^""[0-9]+"","".*"",""[0-9]+\/[0-9]+\/[0-9]+"",""[0-9.]+"",""[0-9.]+"",""[0-9.]+"",""[0-9.]+"".?$";

        [Export("Total", 4)]
        public decimal Total { get; set; }

        [Export("HDL", 5)]
        public decimal HDL { get; set; }

        [Export("LDL", 6)]
        public decimal LDL { get; set; }

        [Export("Triglycerides", 7)]
        public decimal Triglycerides { get; set; }

        public static ExportableCholesterolMeasurement FromCsv(string record)
        {
            string[] words = record.Split(["\",\""], StringSplitOptions.None);
            return new ExportableCholesterolMeasurement
            {
                PersonId = int.Parse(words[0].Replace("\"", "").Trim()),
                Name = words[1].Replace("\"", "").Trim(),
                Date = DateTime.ParseExact(words[2].Replace("\"", "").Trim(), DateTimeFormat, CultureInfo.CurrentCulture),
                Total = decimal.Parse(words[3].Replace("\"", "").Trim()),
                HDL = decimal.Parse(words[4].Replace("\"", "").Trim()),
                LDL = decimal.Parse(words[5].Replace("\"", "").Trim()),
                Triglycerides = decimal.Parse(words[6].Replace("\"", "").Trim())
            };
        }
    }
}

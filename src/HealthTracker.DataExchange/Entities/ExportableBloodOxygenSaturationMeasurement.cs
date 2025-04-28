using HealthTracker.DataExchange.Attributes;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportableBloodOxygenSaturationMeasurement : ExportableMeasurementBase
    {
        public const string CsvRecordPattern = @"^""[0-9]+"","".*"",""[0-9]+\/[0-9]+\/[0-9]+"",""[0-9\.]+"","".*"".?$";

        [Export("Percentage", 4)]
        public decimal Percentage { get; set; }

        [Export("Assessment", 5)]
        public string Assessment { get; set; }

        public static ExportableBloodOxygenSaturationMeasurement FromCsv(string record)
        {
            string[] words = record.Split(["\",\""], StringSplitOptions.None);
            return new ExportableBloodOxygenSaturationMeasurement
            {
                PersonId = int.Parse(words[0].Replace("\"", "").Trim()),
                Name = words[1].Replace("\"", "").Trim(),
                Date = DateTime.ParseExact(words[2].Replace("\"", "").Trim(), DateTimeFormat, CultureInfo.CurrentCulture),
                Percentage = decimal.Parse(words[3].Replace("\"", "").Trim()),
                Assessment = words[4].Replace("\"", "").Trim()
            };
        }
    }
}

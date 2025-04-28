using HealthTracker.DataExchange.Attributes;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportableBloodGlucoseMeasurement : ExportableMeasurementBase
    {
        public const string CsvRecordPattern = @"^""[0-9]+"","".*"",""[0-9]+-[0-9]+-[0-9]+ [0-9]+:[0-9]+:[0-9]+"",""[0-9\.]+"".?$";

        [Export("Level", 4)]
        public decimal Level { get; set; }

        [Export("Assessment", 5)]
        public string Assessment { get; set; }

        public static ExportableBloodGlucoseMeasurement FromCsv(string record)
        {
            string[] words = record.Split(["\",\""], StringSplitOptions.None);
            return new ExportableBloodGlucoseMeasurement
            {
                PersonId = int.Parse(words[0].Replace("\"", "").Trim()),
                Name = words[1].Replace("\"", "").Trim(),
                Date = DateTime.ParseExact(words[2].Replace("\"", "").Trim(), TimestampFormat, CultureInfo.CurrentCulture),
                Level = decimal.Parse(words[3].Replace("\"", "").Trim())
            };
        }
    }
}

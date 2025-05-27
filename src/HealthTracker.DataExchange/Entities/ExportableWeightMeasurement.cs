using HealthTracker.DataExchange.Attributes;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;
using DocumentFormat.OpenXml.InkML;

namespace HealthTracker.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportableWeightMeasurement : ExportableMeasurementBase
    {
        public const string CsvRecordPattern = @"^""[0-9]+"","".*"",""[0-9]+-[A-Za-z]+-[0-9]+ [0-9]+:[0-9]+:[0-9]+"",""[0-9.]+"",""[0-9.]+"","".*"",""[0-9.]+"".?$";

        [Export("Weight", 4)]
        public decimal Weight { get; set; }

        [Export("BMI", 5)]
        public decimal BMI { get; set; }

        [Export("Assessment", 6)]
        public string BMIAssessment { get; set; }

        [Export("BMR", 7)]
        public decimal BMR { get; set; }

        public static ExportableWeightMeasurement FromCsv(string record)
        {
            string[] words = record.Split(["\",\""], StringSplitOptions.None);
            return new ExportableWeightMeasurement
            {
                PersonId = int.Parse(words[0].Replace("\"", "").Trim()),
                Name = words[1].Replace("\"", "").Trim(),
                Date = DateTime.ParseExact(words[2].Replace("\"", "").Trim(), TimestampFormat, CultureInfo.CurrentCulture),
                Weight = decimal.Parse(words[3].Replace("\"", "").Trim()),
                BMI = decimal.Parse(words[4].Replace("\"", "").Trim()),
                BMIAssessment = words[5].Replace("\"", "").Trim(),
                BMR = decimal.Parse(words[6].Replace("\"", "").Trim())
            };
        }
    }
}

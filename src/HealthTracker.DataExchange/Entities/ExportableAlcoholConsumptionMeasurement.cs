using HealthTracker.DataExchange.Attributes;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;
using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportableAlcoholConsumptionMeasurement : ExportableMeasurementBase
    {
        public const string CsvRecordPattern = @"^""[0-9]+"","".*"",""[0-9]+-[A-Za-z]+-[0-9]+ [0-9]+:[0-9]+:[0-9]+"",""[0-9.]+"","".*"",""[0-9]+"","".*"",""[0-9.]+"",""[0-9]+"",""[0-9.]+"".?$";

        [Export("Beverage Id", 4)]
        public int BeverageId { get; set; }

        [Export("Beverage", 5)]
        public string Beverage { get; set; }

        [Export("Measure", 6)]
        public int Measure { get; set; }

        [Export("Measure Name", 7)]
        public string MeasureName { get; set; }

        [Export("Quantity", 8)]
        public int Quantity { get; set; }

        [Export("ABV", 9)]
        public decimal ABV { get; set; }

        [Export("Units", 10)]
        public decimal Units { get; set; }

        public static ExportableAlcoholConsumptionMeasurement FromCsv(string record)
        {
            string[] words = record.Split(["\",\""], StringSplitOptions.None);
            return new ExportableAlcoholConsumptionMeasurement
            {
                PersonId = int.Parse(words[0].Replace("\"", "").Trim()),
                Name = words[1].Replace("\"", "").Trim(),
                Date = DateTime.ParseExact(words[2].Replace("\"", "").Trim(), TimestampFormat, CultureInfo.CurrentCulture),
                BeverageId = int.Parse(words[3].Replace("\"", "").Trim()),
                Beverage = words[4].Replace("\"", "").Trim(),
                Measure = int.Parse(words[5].Replace("\"", "").Trim()),
                MeasureName = words[6].Replace("\"", "").Trim(),
                Quantity = int.Parse(words[7].Replace("\"", "").Trim()),
                ABV = decimal.Parse(words[8].Replace("\"", "").Trim()),
                Units = decimal.Parse(words[9].Replace("\"", "").Trim())
            };
        }
    }
}

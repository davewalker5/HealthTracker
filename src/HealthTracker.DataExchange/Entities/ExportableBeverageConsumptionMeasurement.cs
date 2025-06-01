using HealthTracker.DataExchange.Attributes;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportableBeverageConsumptionMeasurement : ExportableMeasurementBase
    {
        public const string CsvRecordPattern = @"^""[0-9]+"","".*"",""[0-9]+-[A-Za-z]+-[0-9]+ [0-9]+:[0-9]+:[0-9]+"",""[0-9]+"","".*"",""[0-9]+"",""[0-9.]+"",""[0-9.]+"",""[0-9.]+"".?$";

        [Export("Beverage Id", 4)]
        public int BeverageId { get; set; }

        [Export("Beverage", 5)]
        public string Beverage { get; set; }

        [Export("Quantity", 6)]
        public int Quantity { get; set; }

        [Export("Volume", 7)]
        public decimal Volume { get; set; }

        [Export("ABV", 8)]
        public decimal ABV { get; set; }

        [Export("Units", 9)]
        public decimal Units { get; set; }

        public static ExportableBeverageConsumptionMeasurement FromCsv(string record)
        {
            string[] words = record.Split(["\",\""], StringSplitOptions.None);
            return new ExportableBeverageConsumptionMeasurement
            {
                PersonId = int.Parse(words[0].Replace("\"", "").Trim()),
                Name = words[1].Replace("\"", "").Trim(),
                Date = DateTime.ParseExact(words[2].Replace("\"", "").Trim(), TimestampFormat, CultureInfo.CurrentCulture),
                BeverageId = int.Parse(words[3].Replace("\"", "").Trim()),
                Beverage = words[4].Replace("\"", "").Trim(),
                Quantity = int.Parse(words[5].Replace("\"", "").Trim()),
                Volume = decimal.Parse(words[6].Replace("\"", "").Trim()),
                ABV = decimal.Parse(words[7].Replace("\"", "").Trim()),
                Units = decimal.Parse(words[8].Replace("\"", "").Trim())
            };
        }
    }
}

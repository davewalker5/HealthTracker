using HealthTracker.DataExchange.Attributes;
using System;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportablePerson : ExportableEntityBase
    {
        public const string CsvRecordPattern = @"^("".*"",){2}""[0-9]+\/[0-9]+\/[0-9]+"",""[0-9.]+"","".*"".?$";

        [Export("First Names", 1)]
        public string FirstNames { get; set; }

        [Export("Surname", 2)]
        public string Surname { get; set; }

        [Export("Date Of Birth", 3)]
        public DateTime DateOfBirth { get; set; }

        [Export("Height", 4)]
        public decimal Height { get; set; }

        [Export("Gender", 5)]
        public string Gender { get; set; }

        public static ExportablePerson FromCsv(string record)
        {
            string[] words = record.Split(["\",\""], StringSplitOptions.None);
            return new ExportablePerson
            {
                FirstNames = words[0].Replace("\"", "").Trim(),
                Surname = words[1].Replace("\"", "").Trim(),
                DateOfBirth = DateTime.ParseExact(words[2].Replace("\"", "").Trim(), DateTimeFormat, CultureInfo.CurrentCulture),
                Height = decimal.Parse(words[3].Replace("\"", "").Trim()),
                Gender = words[4].Replace("\"", "").Trim(),
            };
        }
    }
}

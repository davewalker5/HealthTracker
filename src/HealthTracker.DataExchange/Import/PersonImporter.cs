using HealthTracker.Entities.Interfaces;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Interfaces;
using HealthTracker.Enumerations.Enumerations;

namespace HealthTracker.DataExchange.Import
{
    public sealed class PersonImporter : CsvImporter<ExportablePerson>, IPersonImporter
    {
        private readonly string[] _validGenders = [
            Gender.Unspecified.ToString(),
            Gender.Male.ToString(),
            Gender.Female.ToString()
        ];

        public PersonImporter(IHealthTrackerFactory factory, string format) : base(factory, format)
        {

        }

        /// <summary>
        /// Inflate a record to an object
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        protected override ExportablePerson Inflate(string record)
            => ExportablePerson.FromCsv(record);

        /// <summary>
        /// Validate an inflated record
        /// </summary>
        /// <param name="person"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
#pragma warning disable CS1998
        protected override void Validate(ExportablePerson person, int recordCount)
        {
            ValidateField<string>(x => !string.IsNullOrEmpty(x), person.FirstNames,  "FirstNames", recordCount);
            ValidateField<string>(x => !string.IsNullOrEmpty(x), person.Surname, "Surname", recordCount);
            ValidateField<DateTime>(x => x <= DateTime.Now, person.DateOfBirth, "DateOfBirth", recordCount);
            ValidateField<decimal>(x => x > 0, person.Height, "Height", recordCount);
            ValidateField<string>(x => _validGenders.Contains(x), person.Gender, "Gender", recordCount);
        }
#pragma warning restore CS1998

        /// <summary>
        /// Store an inflated record in the database
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        protected override async Task AddAsync(ExportablePerson person)
            => await _factory.People.AddAsync(
                person.FirstNames,
                person.Surname,
                person.DateOfBirth,
                person.Height,
                Enum.Parse<Gender>(person.Gender));
    }
}
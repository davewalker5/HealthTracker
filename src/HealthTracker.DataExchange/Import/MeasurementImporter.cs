using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Identity;
using HealthTracker.DataExchange.Interfaces;
using HealthTracker.DataExchange.Entities;

namespace HealthTracker.DataExchange.Import
{
    public abstract class MeasurementImporter<T> : CsvImporter<T>, ICsvImporter<T> where T : ExportableMeasurementBase
    {
        private List<Person> _people = [];

        public MeasurementImporter(IHealthTrackerFactory factory, string format) : base(factory, format)
        {
            
        }

        /// <summary>
        /// Prepare for import
        /// </summary>
        /// <returns></returns>
        protected override async Task Prepare()
            => _people = await _factory.People.ListAsync(x => true, 1, int.MaxValue);

        /// <summary>
        /// Validate an inflated record
        /// </summary>
        /// <param name="measurement"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        protected void ValidateCommonFields(T measurement, int recordCount)
        {
            var person = _people.FirstOrDefault(x => x.Id == measurement.PersonId);
            ValidateField<Person>(x => x != null, person, "PersonId", recordCount);
            ValidateField<string>(x => !string.IsNullOrEmpty(x), measurement.Name, "Name", recordCount);
            ValidateField<DateTime>(x => x <= DateTime.Now, measurement.Date, "Date", recordCount);
        }
    }
}
using HealthTracker.Logic.Factory;
using HealthTracker.DataExchange.Interfaces;
using HealthTracker.DataExchange.Entities;
using HealthTracker.DataExchange.Exceptions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HealthTracker.DataExchange.Import
{
    public abstract class CsvImporter<T> : ICsvImporter<T> where T : class
    {
        private readonly Regex _regex;

        public event EventHandler<ImportEventArgs<T>> RecordImport;

        public CsvImporter(string format)
            => _regex = new Regex(format, RegexOptions.Compiled);

        /// <summary>
        /// Import a collection of CSV format records
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        /// <exception cref="InvalidRecordFormatException"></exception>
        public async Task ImportAsync(IEnumerable<string> records)
        {
            // Perform pre-import preparation
            await Prepare();

            // Import is a two-pass process. The first pass performs validation only. The
            // second imports the data. Aside from storage related errors, this should ensure
            // import either works for all records or fails having imported none
            for (int pass = 0; pass < 2; pass++)
            {
                int count = 0;
                foreach (var record in records)
                {
                    count++;
                    if (count > 1)
                    {
                        if (pass == 0)
                        {
                            ValidateRecordFormatAsync(record, count);
                        }
                        else
                        {
                            await ImportRecordAsync(record, count);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Import the contents of the CSV file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="factory"></param>
        public async Task ImportAsync(string file)
            => await ImportAsync(File.ReadAllLines(file));

        /// <summary>
        /// Prepare for import. To be overridden as needed in child classes
        /// </summary>
        /// <returns></returns>
#pragma warning disable CS1998
        protected virtual async Task Prepare()
        {
        }
#pragma warning restore CS1998

        /// <summary>
        /// Inflate a record to an object. This should throw an exception if inflation fails
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        protected abstract T Inflate(string record);

        /// <summary>
        /// Validate an inflated record. This should throw an exception on error
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        protected abstract void ValidateAsync(T entity, int recordCount);

        /// <summary>
        /// Method to store an object in the database
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected abstract Task AddAsync(T entity);

        /// <summary>
        /// Validate a field value, throwing an exception if it's not valid
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="value"></param>
        /// <param name="fieldName"></param>
        /// <param name="recordCount"></param>
        /// <exception cref="Inv"></exception>
        protected void ValidateField<U>(Predicate<U> predicate, U value, string fieldName, int recordCount)
        {
            if (!predicate(value))
            {
                var message = $"Invalid value for '{fieldName}' at record {recordCount}";
                throw new InvalidFieldValueException(message);
            }
        }

        /// <summary>
        /// Check a record matches the required format and can be inflated to an object
        /// </summary>
        /// <param name="record"></param>
        /// <param name="recordCount"></param>
        /// <exception cref="InvalidRecordFormatException"></exception>
        private void ValidateRecordFormatAsync(string record, int recordCount)
        {
            // Check the line matches the pattern required for successful import
            bool matches = _regex.Matches(record).Any();
            if (!matches)
            {
                string message = $"Invalid record format at line {recordCount}";
                throw new InvalidRecordFormatException(message);
            }

            // Test inflation to an object and validation of the result
            var entity = Inflate(record);
            ValidateAsync(entity, recordCount);
        }

        /// <summary>
        /// Inflate a record to an entity and store it in the database
        /// </summary>
        /// <param name="record"></param>
        /// <param name="recordCount"></param>
        private async Task ImportRecordAsync(string record, int recordCount)
        {
            T entity = Inflate(record);
            await AddAsync(entity);
            RecordImport?.Invoke(this, new ImportEventArgs<T> { RecordCount = recordCount - 1, Entity = entity });
        }
    }
}

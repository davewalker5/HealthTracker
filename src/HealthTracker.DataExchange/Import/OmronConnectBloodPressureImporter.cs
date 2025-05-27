using HealthTracker.Entities.Interfaces;
using HealthTracker.Entities.Identity;
using HealthTracker.DataExchange.Interfaces;
using ClosedXML.Excel;

namespace HealthTracker.DataExchange.Import
{
    public class OmronConnectBloodPressureImporter : IOmronConnectBloodPressureImporter
    {
        private readonly string _dateColumnTitle;
        private readonly string _systolicColumnTitle;
        private readonly string _diastolicColumnTitle;
        private readonly IHealthTrackerFactory _factory;
        private readonly IBloodPressureMeasurementImporter _importer;

        public OmronConnectBloodPressureImporter(
            IHealthTrackerFactory factory,
            IBloodPressureMeasurementImporter importer,
            string dateColumnTitle,
            string systolicColumnTitle,
            string diastolicColumnTitle)
        {
            _dateColumnTitle = dateColumnTitle;
            _systolicColumnTitle = systolicColumnTitle;
            _diastolicColumnTitle = diastolicColumnTitle;
            _factory = factory;
            _importer = importer;
        }

        /// <summary>
        /// Import an OMRON Connect formatted XLSX file represented by a base 64 encoded string of its content
        /// </summary>
        /// <param name="encoded"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        public async Task ImportAsync(string encoded, int personId)
            => await ImportAsync(Convert.FromBase64String(encoded), personId);

        /// <summary>
        /// Import an OMRON Connect formatted XLSX file represented by a byte array of its content
        /// </summary>
        /// <param name="content"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        public async Task ImportAsync(byte[] content, int personId)
        {
            // Retrieve the details of the person
            var people = await _factory.People.ListAsync(x => x.Id == personId, 1, int.MaxValue);
            var person = people.First();

            // Generate CSV format content and import the data
            var records = GenerateCsvContent(content, person);
            await _importer.ImportAsync(records);
        }

        /// <summary>
        /// Generate CSV-format content for all records in the specified content array. That array
        /// contains the content of an XLSX file
        /// </summary>
        /// <param name="content"></param>
        /// <param name="person"></param>
        private IEnumerable<string> GenerateCsvContent(byte[] content, Person person)
        {
            List<string> records = [""];

            using (var stream = new MemoryStream(content))
            {
                using (var workbook = new XLWorkbook(stream))
                {
                    // The data is assumed to be in the first worksheet
                    var worksheet = workbook.Worksheet(1);
                    var headerRow = worksheet.FirstRowUsed();

                    // Get the indices of the columns of interest
                    (int dateIndex, int systolicIndex, int diastolicIndex) = IdentifyColumns(headerRow);

                    // Extract the CSV-formatted data
                    foreach (IXLRow row in worksheet.RowsUsed().Skip(1)) 
                    {
                        var record = GenerateCsvRow(row, dateIndex, systolicIndex, diastolicIndex, person);
                        records.Add(record);
                    } 
                }
            }

            return records;
        }

        /// <summary>
        /// Identify the positions of the columns containing the data to be imported
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private (int dateIndex, int systolicIndex, int diastolicIndex) IdentifyColumns(IXLRow row)
        {
            // Get a list of the string representations of the cell values
            var values = row.CellsUsed().ToList().Select(x => x.Value.ToString()).ToList();

            // Find the indices of the columns of interest
            var dateIndex = values.IndexOf(_dateColumnTitle);
            var systolicIndex = values.IndexOf(_systolicColumnTitle);
            var diastolicIndex = values.IndexOf(_diastolicColumnTitle);

            return (dateIndex, systolicIndex, diastolicIndex);
        }

        /// <summary>
        /// Generate a CSV-format record from a row in the Excel worksheet
        /// </summary>
        /// <param name="row"></param>
        /// <param name="dateIndex"></param>
        /// <param name="systolicIndex"></param>
        /// <param name="diastolicIndex"></param>
        /// <param name="person"></param>
        /// <returns></returns>
        private string GenerateCsvRow(IXLRow row, int dateIndex, int systolicIndex, int diastolicIndex, Person person)
        {
            // Get a list of the string representations of the cell values
            var values = row.CellsUsed().ToList().Select(x => x.Value.ToString()).ToList();

            // Extract the measurement date and values
            var date = ExtractMeasurementDate(values[dateIndex]);
            var systolic = values[systolicIndex];
            var diastolic = values[diastolicIndex];

            // Return a CSV-format record with all fields quoted
            return $"\"{person.Id}\",\"{person.FirstNames} {person.Surname}\",\"{date:dd-MMM-yyyy HH:mm:ss}\",\"{systolic}\",\"{diastolic}\",\"\"";
        }

        /// <summary>
        /// Extract the measurement date from the date column
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private static DateTime ExtractMeasurementDate(string date)
        {
            DateTime extracted;

            // Remove the time and trim the result
            var dateString = date.Split(' ')[0].Trim();

            try
            {
                // Attempt conversion without modification
                extracted = DateTime.Parse(dateString);
            }
            catch (FormatException)
            {
                // Assume the day and month are the wrong way round: Swap them and try again
                var words = dateString.Split('/');
                dateString = $"{words[1]}/{words[0]}/{words[2]}";
                extracted = DateTime.Parse(dateString);
            }

            return extracted;
        }
    }
}
